using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
		private readonly ApiCredentials _apiAcatCred;

		private readonly RestClient _restClient = new();

		private readonly ILogger _logger;

		private ApmTokenResponse _apmApiToken;
		private ApecTokenResponse _apecApiToken;
		private string _acatToken;

		public ApiDataService(IConfiguration config, ILogger<ApiDataService> logger)
		{
			_logger = logger;
			
			_apiApmCred = GetApiCredentials(config.GetSection("apmApi"));
			_apiEmexCred = GetApiCredentials(config.GetSection("emexApi"));
			_apiApecCred = GetApiCredentials(config.GetSection("apecApi"));
			_apiAcatCred = GetApiCredentials(config.GetSection("acatApi"));

			_acatToken = config.GetSection("acatApi")["token"];
		}

		private static ApiCredentials GetApiCredentials(IConfigurationSection section)
		{
			return new ApiCredentials()
			{
				url = section["url"],
				username = section["username"],
				password = section["password"],
				contractNumber = section["contractNumber"]
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

		public async Task<IEnumerable<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode, bool includeAnalogs)
		{
			var result = new List<StandardProductModel>();

			var tasks = new Task<IEnumerable<StandardProductModel>>[3] {
				GetApmDataAsync(detailCode, includeAnalogs),
				GetApecDataAsync(detailCode, includeAnalogs),
				GetEmexDataAsync(detailCode, includeAnalogs)
			};

			try
			{
				var products = await Task.WhenAll(tasks);

				result.AddRange(products[0]);
				result.AddRange(products[1]);
				result.AddRange(products[2]);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while getting API data. PN={0}", detailCode);
			}

			return result;
		}

		private async Task<IEnumerable<StandardProductModel>> GetEmexDataAsync(string detailNum, bool includeAnalogs)
		{
			try
			{
				EmexServiceReference.ServiceSoapClient client;
				EmexServiceReference.Customer customer = null;

				client = new EmexServiceReference.ServiceSoapClient(new EmexServiceReference.ServiceSoapClient.EndpointConfiguration());

				customer = await client.LoginAsync(new EmexServiceReference.Customer()
				{
					UserName = _apiEmexCred.username,
					Password = _apiEmexCred.password
				});

				EmexServiceReference.FindByNumber[] result;

				result = await client.SearchPartAsync(customer, detailNum, true);

				_ = client.CloseAsync();

				return result.ToStandard();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while getting Emex API data. PN={0}", detailNum);

				return new List<StandardProductModel>();
			}
		}

		private async Task<IEnumerable<StandardProductModel>> GetApmDataAsync(string detailCode, bool includeAnalogs)
		{
			try
			{
				var token = await GetApmApiTokenAsync();

				var dataSearch = new
				{
					token.name,
					token.token,
					code = detailCode,
					analogs = includeAnalogs
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while getting Apm API data. PN={0}", detailCode);
				
				return new List<StandardProductModel>();
			}
		}

		private static RestRequest GetRestRequestBearerToken(string? resource, string token, Method method = Method.Get)
		{
			var request = new RestRequest(resource, method);

			request.AddHeader("Authorization", $"Bearer {token}");

			return request;
		}
		private static RestRequest GetRestRequest(string? resource, string token, Method method = Method.Get)
		{
			var request = new RestRequest(resource, method);

			request.AddHeader("Authorization", token);

			return request;
		}

		private async Task<IEnumerable<StandardProductModel>> GetApecDataAsync(string detailCode, bool includeAnalogs)
		{
			var result = new List<StandardProductModel>();

			try
			{
				var token = await GetApecApiTokenAsync();

				var request = GetRestRequestBearerToken($"{_apiApecCred.url}api/getdeliverypoints", token, Method.Get);
				var response = await _restClient.ExecuteAsync(request);
				var firstDeliveryPointId = JsonConvert.DeserializeObject<ApecDeliveryPoint[]>(response.Content)?.First().DeliveryPointID;

				request = GetRestRequestBearerToken(
					$"{_apiApecCred.url}api/search/{detailCode}/brands?deliveryPointID={firstDeliveryPointId}&analogues={includeAnalogs}",
					token,
					Method.Get
				);
				response = await _restClient.ExecuteAsync(request);

				if (string.IsNullOrEmpty(response.Content) || response.Content == "\"Ничего не найдено\"")
				{
					return result;
				}

				var brands = JsonConvert.DeserializeObject<ApecBrand[]>(response.Content);

				foreach (var brand in brands)
				{
					//need to be replaced with multi-theard

					request = GetRestRequestBearerToken(
						$"{_apiApecCred.url}api/search/{detailCode}/brand/{brand.Brand}?deliveryPointID={firstDeliveryPointId}&analogues={includeAnalogs}",
						token,
						Method.Get
					);

					response = await _restClient.ExecuteAsync(request);

					result.AddRange(JsonConvert.DeserializeObject<ApecProduct[]>(response.Content)?.ToStandard(brand.Brand));
				}

				return result;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error while getting Apec API data. PN={0}", detailCode);

				return result;
			}
		}

		public List<StandardProductModel> GetProducts()
		{
			throw new NotImplementedException();
		}

		public async Task<string> CreateApmOrderAsync(PreOrderModel preOrder)
		{
			try
			{
				var token = await GetApmApiTokenAsync();

				var orderPosition = new
				{
					id = 0,
					code = preOrder.PartNumber,
					price = 0,
					make = preOrder.Manufacturer,
					priceName = "",
					supplier_id = 0,
					quantity = 0,
					api_reference = "my custom info"
				};

				var delivery = new
				{
					delivery_type = ApmDeliveryType.RussianPost
				};

				var requestBody = new
				{
					token.name,
					token.token,
					test = "ok", // !!!!!
					order = new[] { orderPosition },
					delivery
				};

				var response = await new HttpClient().PostAsJsonAsync($"{_apiApmCred.url}order/create", requestBody);

				var content = await response.Content.ReadFromJsonAsync<ApmCreateOrderResult> ();

				if (!response.IsSuccessStatusCode)
					return content.info;

				//content = content.Replace($"{detailCode}_code", "root");

				//return JsonConvert.DeserializeObject<ApmRootSearchResult>(content)
				//	.RootElement
				//	.mainProducts
				//	.ToStandard();
				return content.info;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while getting Apm API data. PN={0}", 777);

				return "ssss";
			}
		}

		public async Task<string> CreateApecOrderAsync()
		{
			var result =  "okkkkk2";

			try
			{
				var token = await GetApecApiTokenAsync();

				var request = GetRestRequestBearerToken($"{_apiApecCred.url}api/order", token, Method.Post);

				var orderHeadLines = new dynamic[] { 
					new {
						Count = 1,
						Price = 1.23,
						Refrence = "ABC TEST",
						ReactionByCount = 0,
						ReactionByPrice = 0,
						StrictlyThisNumber = true,
						Brand = "DENSO",
						PartNumber = "IK16",
						SupplierID = 9553
					} 
				};
				
				var body = new {
					IsTest = true, // !!!
					ValidationType = 0,
					OrderHeadLines = orderHeadLines,
					ContractID = _apiApecCred.contractNumber,
					CustOrderNum = "!!!ТЕСТОВЫЙ API!!!",
					OrderNotes = "!!!ТЕСТОВЫЙ API!!!"
				};

				request = request
					.AddHeader("Content-Type", "application/json")
					.AddBody(body, ContentType.Json);

				var response = await _restClient.ExecuteAsync(request);

				if (!response.IsSuccessful)
					_logger.LogError(response.Content);

//				var firstDeliveryPointId = JsonConvert.DeserializeObject<ApecDeliveryPoint[]>(response.Content)?.First().DeliveryPointID;

				//var brands = JsonConvert.DeserializeObject<ApecBrand[]>(response.Content);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while creating Apec order");

				return result;
			}
		}

		public async Task<AcatSearchResult> SearchByVinAsync(string vin)
		{
			var request = GetRestRequest($"{_apiAcatCred.url}catalogs/search?text={vin}&lang=ru", _acatToken, Method.Get);

			var response = await _restClient.ExecuteAsync(request);

			var result = JsonConvert.DeserializeObject<AcatSearchResult>(response.Content);

			return result;
		}

		public async Task<AcatGroupResult?> GetAcatGroupsAsync(AcatGroupInfo groupInfo)
		{
			var request = GetRestRequest(
				$"{_apiAcatCred.url}catalogs/groups?type={groupInfo.GroupType}&mark={groupInfo.Mark}&modification={groupInfo.Modification}&model={groupInfo.Model}&group={groupInfo.Group}",
				_acatToken,
				Method.Get);

			var response = await _restClient.ExecuteAsync(request);

			var qqq = JsonConvert.DeserializeObject<AcatGroupResult>(response.Content);


			return qqq;
		}

		public async Task<AcatPartsSearchResult> GetAcatPartsAsync(AcatGroupInfo groupInfo)
		{
			var request = GetRestRequest(
				$"{_apiAcatCred.url}catalogs/parts?type={groupInfo.GroupType}&mark={groupInfo.Mark}&modification={groupInfo.Modification}&model={groupInfo.Model}&group={groupInfo.Group}&parentGroup={groupInfo.ParentGroup}",
				_acatToken,
				Method.Get);

			var response = await _restClient.ExecuteAsync(request);

			var qqq = JsonConvert.DeserializeObject<AcatPartsSearchResult>(response.Content);

			return qqq;

		}
	}

	
}
