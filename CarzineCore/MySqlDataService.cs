using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace CarzineCore
{
	public class MySqlDataService : IDbDataService, IDbUserService, IDbTranslationService, IDbActionLogService
	{
		private readonly string _connectionString;

		const string _sqlInsertPreOrder =
			"INSERT INTO pre_order(date, pn, manufacturer, price_rub, delivery_min, source_id," +
				"weight, volume, supplyer_price, delivery_cost, extra_charge, supplyer_status, client_status, user_email) " +
			"VALUES (now(), @pn, @manufacturer, @priceRub, @deliveryMin, @sourceId," +
				"@weight, @volume, @supplyerPrice, @deliveryCost, @extraCharge, @supplyerStatus, @clientStatus, @userName)";

		const string _sqlGetPreorders =
			"SELECT * FROM pre_order";

		const string _sqlGetPreorderById =
			"SELECT * FROM pre_order WHERE id = @id";

		const string _sqlGetPreordersByUser =
			"SELECT * FROM pre_order WHERE user_email = @userEmail";

		const string _sqlInsertUser =
			"INSERT INTO user(login_name, pwd, phone) " +
			"VALUES (@loginName, @pwd, @phone)";

		const string _sqlGetUserByName =
			"SELECT u.*," +
				"case isnull(ua.login) when 1 then 0 else 1 end as is_admin "+
			"FROM user u " +
				"LEFT JOIN user_admin ua ON u.login_name = ua.login " +
			"WHERE login_name = @name";

		const string _sqlSetPreOrderClientStatus =
			"UPDATE pre_order " +
			"SET client_status = @status " +
			"WHERE id = @id";

		const string _sqlGetClientStatuses =
			"SELECT * FROM client_order_status";

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

		public MySqlDataService(IConfiguration config)
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
					sourceId = (int)(product.SourceId),
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

		public async Task<PreOrderDto> GetPreOrderAsync(int preOrderId)
		{
			using var connection = GetConnection();

			return await connection.QuerySingleAsync<PreOrderDto>(_sqlGetPreorderById, new { id = preOrderId });
		}

		public async Task<IEnumerable<PreOrderDto>> GetPreOrdersAsync()
		{
			try
			{
				using var connection = GetConnection();

				return await connection.QueryAsync<PreOrderDto>(_sqlGetPreorders);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<IEnumerable<PreOrderDto>> GetPreOrdersByUserAsync(string userEmail)
		{
			try
			{
				using var connection = GetConnection();

				return await connection.QueryAsync<PreOrderDto>(_sqlGetPreordersByUser, new { userEmail });
			}
			catch (Exception ex)
			{
				throw;
			}
		}

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

		public async Task SetPreorderClientStatus(int orderId, ClientStatus status)
		{
			using var connection = GetConnection();

			await connection.ExecuteAsync(_sqlSetPreOrderClientStatus, new { id = orderId, status });
		}

		public async Task<IEnumerable<StatusDto>> GetClientStatusesAsync()
		{
			using var connection = GetConnection();

			return await connection.QueryAsync<StatusDto>(_sqlGetClientStatuses);
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
	}
}
