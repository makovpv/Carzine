using Carzine.Models;
using CarzineCore;
using CarzineCore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SearchController : ControllerBase
	{
		private readonly string _apiUrl;
		private readonly string _apiUserName;
		private readonly string _apiPassword;

		public SearchController(ILogger<SearchController> logger, IConfiguration config)
		{
			var apmApiSection = config.GetSection("apmApi");

			_apiUrl = apmApiSection["url"];
			_apiUserName = apmApiSection["username"];
			_apiPassword = apmApiSection["password"];
		}

		[HttpGet]
		public async Task<List<ProductModel>> Get(string code)
		{
			var httpClient = new HttpClient();

			var data = new
			{
				username = _apiUserName,
				password = _apiPassword
			};

			var response = await httpClient.PostAsJsonAsync($"{_apiUrl}token", data);

			var ttt = await response.Content.ReadFromJsonAsync<ApmTokenResponse>();

			var dataSearch = new
			{
				name = ttt.name,
				token = ttt.token,
				code = code,
				//analogs = false
			};

			var result = await httpClient.PostAsJsonAsync($"{_apiUrl}product/search", dataSearch);

			var ee = await result.Content.ReadAsStringAsync();

			ee = ee.Replace($"{code}_code", "root");

			var mm = JsonConvert.DeserializeObject<ApmRootSearchResult>(ee);

			var myLogic = new CarzineCalculator();

			var res = myLogic.MakePrice(mm.RootElement.mainProducts);

			return res;
		}
	}
}
