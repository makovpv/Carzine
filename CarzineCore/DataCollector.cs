using CarzineCore.Models;
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
	public class DataCollector
	{
		public async Task<decimal> GetCbrCursAsync(string currencyCode = "USD")
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

		public static async Task<IEnumerable<StandardProductModel>> GetEmexDataAsync(string detailNum, ApiCredentials apiCredentials)
		{
			var client = new EmexServiceReference.ServiceSoapClient(new EmexServiceReference.ServiceSoapClient.EndpointConfiguration());

			var customer = await client.LoginAsync(new EmexServiceReference.Customer() {
				UserName = apiCredentials.username,
				Password = apiCredentials.password
			});

			var bbb = await client.SearchPartAsync(customer, detailNum, true);

			_ = client.CloseAsync();

			return bbb.ToStandard();
		}

		public static async Task<IEnumerable<StandardProductModel>> GetApmDataAsync(string code, ApiCredentials apiCredentials)
		{
			var httpClient = new HttpClient();

			var data = new
			{
				username = apiCredentials.username,
				password = apiCredentials.password
			};

			var response = await httpClient.PostAsJsonAsync($"{apiCredentials.url}token", data);

			var ttt = await response.Content.ReadFromJsonAsync<ApmTokenResponse>();

			var dataSearch = new
			{
				name = ttt.name,
				token = ttt.token,
				code = code,
				//analogs = false
			};

			var result = await httpClient.PostAsJsonAsync($"{apiCredentials.url}product/search", dataSearch);

			var ee = await result.Content.ReadAsStringAsync();

			ee = ee.Replace($"{code}_code", "root");

			var mm = JsonConvert.DeserializeObject<ApmRootSearchResult>(ee);

			return mm.RootElement.mainProducts.ToStandard();
		}

		public static async Task<List<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode, ApiCredentials apmCreds, ApiCredentials emexCreds)
		{
			var result = new List<StandardProductModel>();

			result.AddRange(await GetApmDataAsync(detailCode, apmCreds));

			result.AddRange(await GetEmexDataAsync(detailCode, emexCreds));

			return result;
		}
	}
}
