using CarzineCore;
using CarzineCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SearchController : ControllerBase
	{
		private readonly ApiCredentials _apiApm;
		private readonly ApiCredentials _apiEmex;

		public SearchController(ILogger<SearchController> logger, IConfiguration config)
		{
			_apiApm = config.GetSection("apmApi").Get<ApiCredentials>();
			_apiEmex = config.GetSection("emexApi").Get<ApiCredentials>();
		}

		[HttpGet]
		public async Task<List<StandardProductModel>> Get(string code)
		{
			var result = await DataCollector.GetDataMultipleSourceAsync(code, _apiApm, _apiEmex);

			var myLogic = new CarzineCalculator();

			var res = await myLogic.CalcPriceRubAsync(result);

			return res;
		}
	}
}
