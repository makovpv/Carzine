using Carzine.Models;
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

		private readonly ILogger<SearchController> _logger;

		public SearchController(ILogger<SearchController> logger, IConfiguration config)
		{
			_logger = logger;
			
			_apiApm = config.GetSection("apmApi").Get<ApiCredentials>();
			_apiEmex = config.GetSection("emexApi").Get<ApiCredentials>();
		}

		[HttpGet]
		public async Task<IActionResult> Get(string code, int mode = 1)
		{
			List<StandardProductModel> result;
			
			try
			{
				result = mode == 1 ?
				await DataCollector.GetDataSingleSourceAsync(code, _apiApm) :
				await DataCollector.GetDataMultipleSourceAsync(code, _apiApm, _apiEmex);
			}
			catch (Exception ex)
			{
				_logger.LogInformation("******");
				_logger.LogInformation(ex.Message);
				_logger.LogError(ex.StackTrace);
				
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}

			var myLogic = new CarzineCalculator();

			var products = await myLogic.CalcPriceRubAsync(result);

			return StatusCode(
				StatusCodes.Status200OK, 
				new SearchResultViewModel()
				{
					Products = products,
					BestPrice = products.MinBy(x => x.PriceRub),
					ExpressDelivery = products.MinBy(x => x.DeliveryMin)
				});

		}
	}
}
