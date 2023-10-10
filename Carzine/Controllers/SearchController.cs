using Carzine.Models;
using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SearchController : ControllerBase
	{

		private readonly ILogger<SearchController> _logger;

		private readonly IApiDataService _apiDataService;

		public SearchController(ILogger<SearchController> logger, IConfiguration config, IApiDataService apiDataService)
		{
			_logger = logger;

			_apiDataService = apiDataService;
		}

		[HttpGet]
		public async Task<IActionResult> Get(string code, int mode = 1)
		{
			IEnumerable<StandardProductModel> result;
			
			try
			{
				result = mode == 1 ?
					await _apiDataService.GetDataSingleSourceAsync(code) :
					await _apiDataService.GetDataMultipleSourceAsync(code);
			}
			catch (Exception ex)
			{
				_logger.LogInformation("******");
				_logger.LogInformation(ex.Message);
				_logger.LogError(ex.StackTrace);
				
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}

			var myLogic = new CarzineCalculator();

			var products = await myLogic.CalcPriceRubAsync(result.ToList());

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
