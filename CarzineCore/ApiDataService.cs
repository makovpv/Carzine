using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore
{
	public class ApiDataService : IApiDataService
	{
		private readonly ApiCredentials _apiApmCred;
		private readonly ApiCredentials _apiEmexCred;

		private ApmTokenResponse _apmApiToken;

		public ApiDataService(IConfiguration config)
		{
			_apiApmCred = GetApiCredentials(config.GetSection("apmApi"));
			_apiEmexCred = GetApiCredentials(config.GetSection("emexApi"));
		}

		private static ApiCredentials GetApiCredentials(IConfigurationSection section)
		{
			return new ApiCredentials()
			{
				url = section["url"],
				username = section["username"],
				password = section["password"]
			};
		}

		private async Task<ApmTokenResponse> GetApmApiTokenAsync()
		{
			if (_apmApiToken != null)
				return _apmApiToken;

			var data = new
			{
				username = _apiApmCred.username,
				password = _apiApmCred.password
			};

			var response = await new HttpClient().PostAsJsonAsync($"{_apiApmCred.url}token", data);

			_apmApiToken = await response.Content.ReadFromJsonAsync<ApmTokenResponse>();

			return _apmApiToken;
		}

		public async Task<IEnumerable<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode)
		{
			var result = new List<StandardProductModel>();

			var tasks = new Task<IEnumerable<StandardProductModel>>[2] {
				GetApmDataAsync(detailCode),
				GetEmexDataAsync(detailCode)
			};

			var products = await Task.WhenAll(tasks);

			result.AddRange(products[0]);
			result.AddRange(products[1]);

			return result;
		}

		private async Task<IEnumerable<StandardProductModel>> GetEmexDataAsync(string detailNum)
		{
			EmexServiceReference.ServiceSoapClient client;
			EmexServiceReference.Customer customer = null;

			client = new EmexServiceReference.ServiceSoapClient(new EmexServiceReference.ServiceSoapClient.EndpointConfiguration());

			try
			{
				customer = await client.LoginAsync(new EmexServiceReference.Customer()
				{
					UserName = _apiEmexCred.username,
					Password = _apiEmexCred.password
				});
			}
			catch (Exception ex)
			{
				throw new Exception("M2" + ex.Message);
			}

			EmexServiceReference.FindByNumber[] bbb;

			bbb = await client.SearchPartAsync(customer, detailNum, true);

			_ = client.CloseAsync();

			return bbb.ToStandard();
		}

		private async Task<IEnumerable<StandardProductModel>> GetApmDataAsync(string detailCode)
		{
			var token = await GetApmApiTokenAsync();

			var dataSearch = new
			{
				name = token.name,
				token = token.token,
				code = detailCode,
				//analogs = false
			};

			var result = await new HttpClient().PostAsJsonAsync($"{_apiApmCred.url}product/search", dataSearch);

			var content = await result.Content.ReadAsStringAsync();

			content = content.Replace($"{detailCode}_code", "root");

			return JsonConvert.DeserializeObject<ApmRootSearchResult>(content)
				.RootElement
				.mainProducts
				.ToStandard();
		}

		public async Task<IEnumerable<StandardProductModel>> GetDataSingleSourceAsync(string detailCode)
		{
			return await GetApmDataAsync(detailCode);
		}

		public List<StandardProductModel> GetProducts()
		{
			throw new NotImplementedException();
		}
	}
}
