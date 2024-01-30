using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace CarzineCore
{
	public class MySqlDataService : IDbDataService, IDbUserService
	{
		private readonly string _connectionString;

		const string _sqlInsertPreOrder =
			"INSERT INTO pre_order(phone, date, pn, manufacturer, price_rub, delivery_min, source_id," +
				"weight, volume, supplyer_price, delivery_cost, extra_charge, supplyer_status, client_status, user_email) " +
			"VALUES (@phone, now(), @pn, @manufacturer, @priceRub, @deliveryMin, @sourceId," +
				"@weight, @volume, @supplyerPrice, @deliveryCost, @extraCharge, @supplyerStatus, @clientStatus, @userName)";

		const string _sqlGetPreorders =
			"SELECT * FROM pre_order";

		const string _sqlGetPreorderById =
			"SELECT * FROM pre_order WHERE id = @id";

		const string _sqlInsertUser =
			"INSERT INTO user(login_name, pwd) " +
			"VALUES (@loginName, @pwd)";

		const string _sqlGetUserByName =
			"SELECT * FROM user WHERE login_name = @name";

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

		public async Task<int> AddPreOrderAsync(PreOrderModel preorder, string userName)
		{
			int preOrderId;

			try
			{
				using var connection = GetConnection();

				preOrderId = await connection.ExecuteAsync(_sqlInsertPreOrder, new
				{
					phone = preorder.Phone,
					pn = preorder.Product.PartNumber,
					manufacturer = preorder.Product.Manufacturer,
					priceRub = preorder.Product.PriceRub,
					deliveryMin = preorder.Product.DeliveryMin,
					sourceId = (int)(preorder.Product.SourceId),
					weight = preorder.Product.Weight,
					volume = preorder.Product.Volume,
					supplyerPrice = preorder.Product.Price,
					deliveryCost = preorder.Product.DeliveryCost,
					extraCharge = preorder.Product.ExtraCharge,
					supplyerStatus = SupplyerStatus.New,
					clientStatus = ClientStatus.New,
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

		public async Task AddUserAsync(string userName, string pwd)
		{
			try
			{
				using var connection = GetConnection();

				await connection.ExecuteAsync(_sqlInsertUser, new
				{
					loginName = userName,
					pwd = pwd
				});
			}
			catch (Exception ex)
			{
				if (ex.Message.StartsWith("Duplicate entry"))
				{
					throw new Exception("User allready exists");
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
	}
}
