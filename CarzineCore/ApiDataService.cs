using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
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
		private readonly ApiCredentials _apiApecCred;

		private readonly RestClient _restClient = new();

		private ApmTokenResponse _apmApiToken;
		private ApecTokenResponse _apecApiToken;

		public ApiDataService(IConfiguration config)
		{
			_apiApmCred = GetApiCredentials(config.GetSection("apmApi"));
			_apiEmexCred = GetApiCredentials(config.GetSection("emexApi"));
			_apiApecCred = GetApiCredentials(config.GetSection("apecApi"));
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

			if (_apiApmCred == null)
			{
				throw new Exception("Apm credentials is empty");
			}

			var data = new
			{
				username = _apiApmCred.username,
				password = _apiApmCred.password
			};

			var response = await new HttpClient().PostAsJsonAsync($"{_apiApmCred.url}token", data);

			_apmApiToken = await response.Content.ReadFromJsonAsync<ApmTokenResponse>();

			return _apmApiToken;
		}

		private async Task<string> GetApecApiTokenAsync()
		{
			if (_apecApiToken != null && _apecApiToken.expireDate > DateTime.Now)
				return _apecApiToken.access_token;

			var request = new RestRequest($"{_apiApecCred.url}/token", Method.Post);
			request.AddHeader("Content-Type", "text/plain");
			var body = $"username={_apiApecCred.username}&password={_apiApecCred.password}&grant_type=password";
			request.AddParameter("text/plain", body, ParameterType.RequestBody);
			
			var response = await _restClient.ExecuteAsync(request);

			_apecApiToken = JsonConvert.DeserializeObject<ApecTokenResponse>(response.Content);

			_apecApiToken.expireDate = DateTime.Now.AddSeconds(_apecApiToken.expires_in);

			return _apecApiToken.access_token;
		}

		public async Task<IEnumerable<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode)
		{
			var result = new List<StandardProductModel>();

			var tasks = new Task<IEnumerable<StandardProductModel>>[3] {
				GetApmDataAsync(detailCode),
				GetApecDataAsync(detailCode),
				GetEmexDataAsync(detailCode)
			};

			var products = await Task.WhenAll(tasks);

			result.AddRange(products[0]);
			result.AddRange(products[1]);
			result.AddRange(products[2]);

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
				token.name,
				token.token,
				code = detailCode,
				//analogs = false
			};

			var result = await new HttpClient().PostAsJsonAsync($"{_apiApmCred.url}product/search", dataSearch);

			if (!result.IsSuccessStatusCode)
				return new List<StandardProductModel>();

			var content = await result.Content.ReadAsStringAsync();

			content = content.Replace($"{detailCode}_code", "root");

			return JsonConvert.DeserializeObject<ApmRootSearchResult>(content)
				.RootElement
				.mainProducts
				.ToStandard();
		}

		private static RestRequest GetRestRequest(string? resource, string token, Method method = Method.Get)
		{
			var request = new RestRequest(resource, method);

			request.AddHeader("Authorization", $"Bearer {token}");

			return request;
		}

		private async Task<IEnumerable<StandardProductModel>> GetApecDataAsync(string detailCode)
		{
			var token = await GetApecApiTokenAsync();

			var request = GetRestRequest($"{_apiApecCred.url}api/getdeliverypoints", token, Method.Get);
			var response = await _restClient.ExecuteAsync(request);
			var firstDeliveryPointId = JsonConvert.DeserializeObject<ApecDeliveryPoint[]>(response.Content).First().DeliveryPointID;

			request = GetRestRequest($"{_apiApecCred.url}api/search/{detailCode}/brands?deliveryPointID={firstDeliveryPointId}&analogues=false", token, Method.Get);
			response = await _restClient.ExecuteAsync(request);

			if (response.Content == "\"Ничего не найдено\"")
			{
				return new List<StandardProductModel>();
			}

			var firstBrand = JsonConvert.DeserializeObject<ApecBrand[]>(response.Content).First();

			request = GetRestRequest($"{_apiApecCred.url}api/search/{detailCode}/brand/{firstBrand.Brand}?deliveryPointID={firstDeliveryPointId}&analogues=false", token, Method.Get);
			response = await _restClient.ExecuteAsync(request);

			var result = JsonConvert.DeserializeObject<ApecProduct[]>(response.Content).ToStandard();

			return result;
		}

		public async Task<IEnumerable<StandardProductModel>> GetDataSingleSourceAsync(string detailCode)
		{
			var result = new List<StandardProductModel>();

			var tasks = new Task<IEnumerable<StandardProductModel>>[2] {
				GetApmDataAsync(detailCode),
				GetApecDataAsync(detailCode)
			};

			var products = await Task.WhenAll(tasks);

			result.AddRange(products[0]);
			result.AddRange(products[1]);

			return result;
		}

		public List<StandardProductModel> GetProducts()
		{
			throw new NotImplementedException();
		}
	}
}
