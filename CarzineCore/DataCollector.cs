using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CarzineCore
{
	public class DataCollector : IDataCollector
	{
		private readonly IApiDataService _apiDataService;
		private readonly IDbDataRepository _dbDataRepository;
		private readonly ILogger<DataCollector> _logger;

		private readonly Dictionary<int, StandardProductModel> _productDictionary = new();

		public DataCollector(IApiDataService apiDataService, IDbDataRepository dataRepository, ILogger<DataCollector> logger)
		{
			_apiDataService = apiDataService;
			_dbDataRepository = dataRepository;
			_logger = logger;
		}

		private static async Task<decimal> GetCbrCursAsync(string currencyCode = "USD")
		{
			var client = new CbrDailyInfo.DailyInfoSoapClient(new CbrDailyInfo.DailyInfoSoapClient.EndpointConfiguration());

			var result = await client.GetCursOnDateAsync(DateTime.Today);

			var currencyData = result.Nodes[1].Element("ValuteData").Elements().FirstOrDefault(
				x => x.Element("VchCode").Value == currencyCode
			);

			var ci = new CultureInfo("en-US");

			var resultFFF = Convert.ToDecimal(currencyData.Element("Vcurs").Value, ci);

			_ = client.CloseAsync();

			return resultFFF;
		}

		public async Task<List<StandardProductModel>> GetCalculatedDataAsync(string detailCode, bool includeAnalogs)
		{
			var productData = await _apiDataService.GetDataMultipleSourceAsync(detailCode, includeAnalogs);

			var usdRate = await GetCbrCursAsync("USD");

			var products = await new CarzineCalculator(_dbDataRepository).CalcProductComponentsAsync(productData, usdRate);

			foreach (var product in products)
			{
				var hash = product.GetDeterministicHashCode();

				_productDictionary[hash] = product;
			}

			_logger.LogInformation($"Count of hashed products is {_productDictionary.Count()}");

			return products;
		}

		public StandardProductModel GetProductByHash(int hash)
		{
			return _productDictionary[hash];
		}

		//public static void CreateClient()
		//{
		//	var client = new EmexServiceReference.ServiceSoapClient(new EmexServiceReference.ServiceSoapClient.EndpointConfiguration());
		//}

		//public static async Task<string> GetCustomer(ApiCredentials apiCredentials)
		//{
		//	var client = new EmexServiceReference.ServiceSoapClient(new EmexServiceReference.ServiceSoapClient.EndpointConfiguration());

		//	var customer = await client.LoginAsync(new EmexServiceReference.Customer()
		//	{
		//		UserName = apiCredentials.username,
		//		Password = apiCredentials.password
		//	});

		//	return customer.CustomerId;
		//}

		//public static async Task<IEnumerable<StandardProductModel>> GetEmexDataAsync(string detailNum, ApiCredentials apiCredentials)
		//{
		//	EmexServiceReference.ServiceSoapClient client;
		//	EmexServiceReference.Customer customer = null;

		//	client = new EmexServiceReference.ServiceSoapClient(new EmexServiceReference.ServiceSoapClient.EndpointConfiguration());

		//	try
		//	{
		//		customer = await client.LoginAsync(new EmexServiceReference.Customer()
		//		{
		//			UserName = apiCredentials.username,
		//			Password = apiCredentials.password
		//		});
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception("M2" + ex.Message);
		//	}

		//	EmexServiceReference.FindByNumber[] bbb;

		//	bbb = await client.SearchPartAsync(customer, detailNum, true);

		//	_ = client.CloseAsync();

		//	return bbb.ToStandard();
		//}

		//public static async Task<IEnumerable<StandardProductModel>> GetApmDataAsync(string code, ApiCredentials apiCredentials)
		//{
		//	var httpClient = new HttpClient();

		//	var data = new
		//	{
		//		username = apiCredentials.username,
		//		password = apiCredentials.password
		//	};

		//	var response = await httpClient.PostAsJsonAsync($"{apiCredentials.url}token", data);

		//	var ttt = await response.Content.ReadFromJsonAsync<ApmTokenResponse>();

		//	var dataSearch = new
		//	{
		//		name = ttt.name,
		//		token = ttt.token,
		//		code = code,
		//		//analogs = false
		//	};

		//	var result = await httpClient.PostAsJsonAsync($"{apiCredentials.url}product/search", dataSearch);

		//	var ee = await result.Content.ReadAsStringAsync();

		//	ee = ee.Replace($"{code}_code", "root");

		//	var mm = JsonConvert.DeserializeObject<ApmRootSearchResult>(ee);

		//	return mm.RootElement.mainProducts.ToStandard();
		//}

		//public static async Task<List<StandardProductModel>> GetDataSingleSourceAsync(string detailCode, ApiCredentials apmCreds)
		//{
		//	var res = await GetApmDataAsync(detailCode, apmCreds);
		//	return res.ToList();
		//}

		//public static async Task<List<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode, ApiCredentials apmCreds, ApiCredentials emexCreds)
		//{
		//	var result = new List<StandardProductModel>();

		//	var tasks = new Task<IEnumerable<StandardProductModel>>[2] {
		//		GetApmDataAsync(detailCode, apmCreds),
		//		GetEmexDataAsync(detailCode, emexCreds)
		//	};

		//	var products = await Task.WhenAll(tasks);

		//	result.AddRange(products[0]);
		//	result.AddRange(products[1]);

		//	return result;
		//}


	}
}
