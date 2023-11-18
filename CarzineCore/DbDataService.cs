using CarzineCore.Interfaces;
using CarzineCore.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore
{
	public class DbDataService : IDbDataService
	{
		private readonly string _connectionString;

		private readonly ILogger<DbDataService> _logger;

		const string _sqlInsertPreOrder =
			"INSERT INTO pre_order(phone, [date], pn, manufacturer, price_rub, delivery_min, source_id," +
				"[number], weight, volume, supplyer_price, delivery_cost, extra_charge, supplyer_status, client_status) " +
			"VALUES (@phone, getUtcDate(), @pn, @manufacturer, @priceRub, @deliveryMin, @sourceId," +
				"'???', @weight, @volume, @supplyerPrice, @deliveryCost, @extraCharge, @supplyerStatus, @clientStatus)";
		
		const string _sqlGetPreorders =
			"SELECT * FROM pre_order";

		const string _sqlGetPreorderById =
			"SELECT * FROM pre_order WHERE id = @id";


		public DbDataService(IConfiguration config, ILogger<DbDataService> logger)
		{
			_logger = logger;
			
			_connectionString = config.GetConnectionString("carzine");
		}

		private SqlConnection GetConnection()
		{
			var connection = new SqlConnection(_connectionString);

			if (connection.State != ConnectionState.Open)
				connection.Open();

			return connection;
		}

		public async Task<PreOrderModel> GetPreOrderAsync(int preOrderId)
		{
			using var connection = GetConnection();

			var preOrder = await connection.QuerySingleAsync<PreOrderDto>(_sqlGetPreorderById, new { id = preOrderId });

			return new PreOrderModel()
			{
				Id = preOrder.Id,
				PartNumber = preOrder.PN,
				SourceId = (ApiSource)preOrder.source_Id
			};
		}

		public async Task<int> AddPreOrderAsync(PreOrderModel preorder)
		{
			int preOrderId;
			
			try
			{
				using var connection = GetConnection();

				preOrderId = await connection.ExecuteAsync(_sqlInsertPreOrder, new
				{
					phone = preorder.Phone,
					pn = preorder.PartNumber,
					manufacturer = preorder.Manufacturer,
					priceRub = preorder.PriceRub,
					deliveryMin = preorder.DeliveryMin,
					sourceId = (int)preorder.SourceId,
					weight = preorder.Weight,
					volume = preorder.Volume,
					supplyerPrice = 123,
					deliveryCost = 123,
					extraCharge = 123,
					supplyerStatus = 123,
					clientStatus = 123
				});
			}
			catch (Exception ex)
			{
				//LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
				throw;
			}

			return preOrderId;
		}

		protected static DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
		{
			var command = new SqlCommand(commandText, connection as SqlConnection)
			{
				CommandType = commandType
			};

			return command;
		}

		protected object ExecuteScalar(string procedureName, List<SqlParameter> parameters)
		{
			object returnValue = null;

			try
			{
				using var connection = this.GetConnection();

				DbCommand cmd = GetCommand(connection, procedureName, CommandType.StoredProcedure);

				if (parameters != null && parameters.Count > 0)
				{
					cmd.Parameters.AddRange(parameters.ToArray());
				}

				returnValue = cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				//LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
				throw;
			}

			return returnValue;
		}

		public async Task<IEnumerable<PreOrderModel>> GetPreOrdersAsync()
		{
			var result = new List<PreOrderModel>();
			
			try
			{
				using var connection = GetConnection();

				var reader = await connection.ExecuteReaderAsync(_sqlGetPreorders);

				while (reader.Read())
				{
					result.Add(new PreOrderModel
					{
						Phone = reader.GetString("phone"),
						PartNumber = reader.GetString("pn"),
						Manufacturer = reader.GetString("manufacturer"),
						PriceRub = reader.GetDecimal("price_rub"),
						DeliveryMin = reader.GetInt32("delivery_min"),
						Id = reader.GetInt32("id")
					});
				}
			}
			catch (Exception ex)
			{
				//LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
				throw;
			}

			return result;
		}

		//protected DbDataReader GetDataReader(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
		//{
		//	DbDataReader ds;

		//	try
		//	{
		//		DbConnection connection = this.GetConnection();
		//		{
		//			DbCommand cmd = this.GetCommand(connection, procedureName, commandType);
		//			if (parameters != null && parameters.Count > 0)
		//			{
		//				cmd.Parameters.AddRange(parameters.ToArray());
		//			}

		//			ds = cmd.ExecuteReader(CommandBehavior.CloseConnection);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//LogException("Failed to GetDataReader for " + procedureName, ex, parameters);
		//		throw;
		//	}

		//	return ds;
		//}
	}
}
