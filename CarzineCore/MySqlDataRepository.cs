using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace CarzineCore
{
	public class MySqlDataRepository : IDbDataRepository, IDbUserService, IDbTranslationService, IDbActionLogService, IOrderRepository
	{
		private readonly string _connectionString;

		private IEnumerable<RuleRangeDto> _rules;

		#region SQL

		const string _sqlInsertPreOrder =
			"INSERT INTO pre_order(date, pn, manufacturer, price_rub, delivery_min, delivery_min_original, delivery_max_original, " +
				"source_id, weight, volume, supplyer_price, delivery_cost, extra_charge, supplyer_status, client_status, user_email) " +
			"VALUES (now(), @pn, @manufacturer, @priceRub, @deliveryMin, @deliveryMinOriginal, @deliveryMaxOriginal, @sourceId," +
				"@weight, @volume, @supplyerPrice, @deliveryCost, @extraCharge, @supplyerStatus, @clientStatus, @userName)";

		const string _sqlGetOrders =
			"SELECT o.*, u.phone " +
			"FROM `order` o " +
			"LEFT JOIN `user` u ON u.login_name = o.user_email";

		const string _sqlGetOrderById =
			"SELECT o.id, o.user_email, sum(oi.price_rub) AS total_sum " +
			"FROM `order` o " +
			"LEFT JOIN `order_item` oi ON o.id = oi.order_id " +
			"WHERE o.id = @id " +
			"GROUP BY o.id, o.user_email";

		const string _sqlGetOrdersByUser =
			"SELECT o.id, o.date, o.payment_order_state, o.client_status_id, sum(oi.price_rub) as total_sum " +
			"from `order` o " +
			"left join order_item oi on o.id = oi.order_id " +
			"where user_email = @userEmail " +
			"group by o.id, o.date, o.user_email, o.payment_order_state, o.client_status_id;";

		const string _sqlInsertUser =
			"INSERT INTO user(login_name, pwd, phone) " +
			"VALUES (@loginName, @pwd, @phone)";

		const string _sqlGetUserByName =
			"SELECT u.*," +
				"case isnull(ua.login) when 1 then 0 else 1 end as is_admin "+
			"FROM user u " +
				"LEFT JOIN user_admin ua ON u.login_name = ua.login " +
			"WHERE login_name = @name";

		const string _sqlSetOrderClientStatus =
			"UPDATE `order` " +
			"SET client_status_id = @status " +
			"WHERE id = @id";
		const string _sqlSetOrderPaymentOrderId =
			"UPDATE `order` " +
			"SET payment_order_id = @paymentOrderId, " +
				"payment_order_state = @paymentStatus " +
			"WHERE id = @orderId";

		const string _sqlGetClientStatuses =
			"SELECT * FROM client_order_status";

		const string _sqlGetRuleRanges =
			"SELECT * FROM rule_range";
		const string _sqlDeleteRuleRange =
			"DELETE FROM rule_range WHERE id = @id";
		const string _sqlAddRuleRange =
			"INSERT INTO rule_range(min, max, value, type) " +
			"VALUES(@min, @max, @value, @type); " +
			"SELECT LAST_INSERT_ID();";

		const string _sqlGetAllTranslations =
			"SELECT en_name AS enName, ru_name AS ruName FROM translation";
		const string _sqlAddTranslations =
			"INSERT INTO translation(en_name, ru_name) VALUES (@key, @translation)";
		const string _sqlDeleteTranslations =
			"DELETE FROM translation WHERE en_name = @key";

		const string _sqlInsertVinSearchHistory =
			"INSERT INTO vin_search_history(vin, user_name, search_date, " +
				"mark_name, model_name, modification_name, year, " +
				"mark, model_code, modification_code) " +
			"VALUES (@vin, @userName, now(), @markName, @modelName, @modificationName, @year, " +
				"@mark, @modelCode, @modificationCode) " +
			"ON DUPLICATE KEY UPDATE search_date = now();";
		const string _sqlGetUserAuto =
			"SELECT vin, mark_name AS markName, model_name AS modelName, year, " +
				"mark, model_code AS modelCode, modification_code AS modificationCode " +
			"FROM vin_search_history " +
			"WHERE user_name = @userName " +
			"ORDER BY search_date DESC " +
			"LIMIT @limit";

		const string _sqlGetUserCart =
			"SELECT * FROM cart WHERE login = @login";
		const string _sqlGetUserCartItemByHash =
			"SELECT id FROM cart " +
			"WHERE item_hash = @hash AND login = @login";
		const string _sqlAddItemToCart =
			"INSERT INTO cart (item_code, code_type, login, name, amount, price_rub, item_hash," +
				"manufacturer, delivery_min, weight, volume, supplyer_price, delivery_cost, extra_charge," +
				"delivery_min_original, delivery_max_original, source_id) " +
			"VALUES(@itemCode, @codeType, @login, @name, @amount, @priceRub, @hash," +
				"@manufacturer, @deliveryMin, @weight, @volume, @supplyerPrice, @deliveryCost, @extraCharge," +
				"@deliveryMinOriginal, @deliveryMaxOriginal, @sourceId); " +
			"SELECT LAST_INSERT_ID();";
		const string _sqlRemoveUserCartItem =
			"DELETE FROM cart " +
			"WHERE id = @id AND login = @login";
		const string _sqlMakeOrderFromCart =
			"CALL make_order_from_cart(@login)";
		const string _sqlMergeUserCart =
			"CALL merge_user_cart(@uid, @login)";
		#endregion SQL

		public MySqlDataRepository(IConfiguration config)
		{
			_connectionString = config.GetConnectionString("carzineMySql");
		}

		private MySqlConnection GetConnection()
		{
			var connection = new MySqlConnection(_connectionString);

			if (connection.State != ConnectionState.Open)
				connection.Open();

			return connection;
		}

		public async Task<int> AddPreOrderAsync(StandardProductModel product, string userName)
		{
			int preOrderId;

			try
			{
				using var connection = GetConnection();

				preOrderId = await connection.ExecuteAsync(_sqlInsertPreOrder, new
				{
					pn = product.PartNumber,
					manufacturer = product.Manufacturer,
					priceRub = product.PriceRub,

					deliveryMin = product.DeliveryMin,
					deliveryMinOriginal = product.DeliveryMinOriginal,
					deliveryMaxOriginal = product.DeliveryMaxOriginal,

					sourceId = (int)product.SourceId,
					weight = product.Weight,
					volume = product.Volume,
					supplyerPrice = product.Price,
					deliveryCost = product.DeliveryCost,
					extraCharge = product.ExtraCharge,
					supplyerStatus = SupplyerStatus.New,
					clientStatus = ClientStatus.InProgress,
					userName = userName
				});
			}
			catch (Exception ex)
			{
				//LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
				throw;
			}

			return preOrderId;
		}

		public async Task<OrderDto> GetOrderAsync(int orderId)
		{
			using var connection = GetConnection();

			return await connection.QuerySingleAsync<OrderDto>(_sqlGetOrderById, new { id = orderId });
		}

		public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
		{
			try
			{
				using var connection = GetConnection();

				return await connection.QueryAsync<OrderDto>(_sqlGetOrders);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(string userEmail)
		{
			try
			{
				using var connection = GetConnection();

				return await connection.QueryAsync<OrderDto>(_sqlGetOrdersByUser, new { userEmail });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		//public async Task<decimal> GetOrderTotalSumAsync(int orderId)
		//{
		//	try
		//	{
		//		using var connection = GetConnection();

		//		return await connection.QueryAsync<OrderDto>(_sqlGetOrdersByUser, new { userEmail });
		//	}
		//	catch (Exception ex)
		//	{
		//		throw;
		//	}
		//}

		public async Task AddUserAsync(string userName, string pwd, string phone)
		{
			try
			{
				using var connection = GetConnection();

				await connection.ExecuteAsync(_sqlInsertUser, new
				{
					loginName = userName,
					pwd = pwd,
					phone
				});
			}
			catch (Exception ex)
			{
				if (ex.Message.StartsWith("Duplicate entry"))
				{
					throw new CarzineException("User allready exists");
				}
				//LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
				throw;
			}

		}

		public async Task<UserDto?> GetUserByName(string userName)
		{
			try
			{
				using var connection = GetConnection();

				var tt = await connection.QuerySingleOrDefaultAsync<UserDto>(_sqlGetUserByName, new { name = userName });

				return tt;
			}
			catch(Exception ex)
			{
				//"Sequence contains no elements"
				throw;
			}
		}

		public async Task SetOrderClientStatus(int orderId, ClientStatus status)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlSetOrderClientStatus, new { id = orderId, status });
		}

		public async Task SetPaymentOrderIdAsync(int orderId, string paymentOrderId, string paymentStatus)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlSetOrderPaymentOrderId, new { orderId, paymentOrderId, paymentStatus });
		}

		public async Task<IEnumerable<StatusDto>> GetClientStatusesAsync()
		{
			using var connection = GetConnection();

			return await connection.QueryAsync<StatusDto>(_sqlGetClientStatuses);
		}

		public async Task<IEnumerable<RuleRangeDto>> GetRuleRangesAsync()
		{
			if (_rules != null)
			{
				return _rules;
			}
			
			using var connection = GetConnection();

			_rules = await connection.QueryAsync<RuleRangeDto>(_sqlGetRuleRanges);

			return _rules;
		}

		public async Task<IEnumerable<RuleRangeDto>> GetRuleRangesAsync(RuleRangeType rangeType)
		{
			var ranges = await GetRuleRangesAsync();

			return ranges.Where(x => x.Type == rangeType);
		}

		public async Task AddRuleRangeAsync(RuleRangeDto ruleRange)
		{
			using var connection = GetConnection();

			var id = await connection.ExecuteScalarAsync<int>(_sqlAddRuleRange, new
			{
				min = ruleRange.Min,
				max = ruleRange.Max,
				value = ruleRange.Value,
				type = (int)ruleRange.Type
			});

			var newRuleRange = new RuleRangeDto(Id: id, ruleRange.Min, ruleRange.Max, ruleRange.Value, ruleRange.Type);
				
			_rules = _rules.Append(newRuleRange);
		}

		public async Task DeleteRuleRangeAsync(int id)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlDeleteRuleRange, new { id });

			_rules = _rules.Where(x => x.Id != id);
		}

		public async Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync()
		{
			using var connection = GetConnection();

			return await connection.QueryAsync<TranslationDto>(_sqlGetAllTranslations);
		}

		public async Task AddTranslationAsync(string key, string translation)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlAddTranslations, new { key, translation });
		}

		public async Task DeleteTranslationAsync(string key)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlDeleteTranslations, new { key });
		}

		private static int? GetProductionYear(AcatVinParameterModel[] parameters)
		{
			var productionYear = parameters.FirstOrDefault(x => x.key.ToLower() == "year")?.value;

			if (!string.IsNullOrEmpty(productionYear)) {
				return Convert.ToInt16(productionYear);
			}

			var productionDate = parameters.FirstOrDefault(x => x.key.ToLower() == "production date")?.value;

			if (DateOnly.TryParse(productionDate, out DateOnly prdDate))
			{
				return prdDate.Year;
			}

			return null;
		}

		public async Task LogUserAutoAsync (string vin, AcatVinModel acatVin, string userName)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlInsertVinSearchHistory, new
				{
					userName,
					vin,
					acatVin.markName,
					acatVin.modelName,
					modificationName = acatVin.modification,
					acatVin.mark,
					modelCode = acatVin.model,
					modificationCode = acatVin.modification,
					year = GetProductionYear(acatVin.parameters)
			});
		}

		public async Task<IEnumerable<UserAutoDto>> GetUserAutoAsync(string userName, int limit)
		{
			using var connection = GetConnection();

			return await connection.QueryAsync<UserAutoDto>(_sqlGetUserAuto, new { userName, limit });
		}

		public async Task<int> AddToCartAsync(string userName, int hash, StandardProductModel product)
		{
			using var connection = GetConnection();

			var id = await connection.QueryFirstOrDefaultAsync<int?>(_sqlGetUserCartItemByHash, new { 
				hash, 
				login = userName
			});

			if (id.HasValue)
				return id.Value;

			var newId = await connection.ExecuteScalarAsync<int>(_sqlAddItemToCart, new
			{
				hash = hash,
				itemCode = product.PartNumber,
				codeType = product.PartNumberType,
				login = userName,
				name = product.Name,
				amount = 1,
				priceRub = product.PriceRub,
				@manufacturer = product.Manufacturer,
				deliveryMin = product.DeliveryMin,
				weight = product.Weight,
				volume = product.Volume,
				supplyerPrice = product.Price,
				deliveryCost = product.DeliveryCost,
				extraCharge = product.ExtraCharge,
				deliveryMinOriginal = product.DeliveryMinOriginal,
				deliveryMaxOriginal = product.DeliveryMaxOriginal,
				sourceId = product.SourceId
			});

			return newId;
		}

		public async Task<int> MakeOrderFromCartAsync(string userName)
		{
			using var connection = GetConnection();

			return await connection.ExecuteScalarAsync<int>(_sqlMakeOrderFromCart, new { login = userName });
		}

		public async Task<int> MergeUserCartAsync(string uid, string userName)
		{
			using var connection = GetConnection();

			return await connection.ExecuteScalarAsync<int>(_sqlMergeUserCart, new { uid, login = userName });
		}

		public async Task RemoveFromCartAsync(int id, string login)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlRemoveUserCartItem, new { id, login });
		}

		public async Task<IEnumerable<CartItemDto>> GetCartAsync(string userName)
		{
			using var connection = GetConnection();

			return await connection.QueryAsync<CartItemDto>(_sqlGetUserCart, new { login = userName });
		}
	}
}
