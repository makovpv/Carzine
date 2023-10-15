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

		public SearchController(ILogger<SearchController> logger, IApiDataService apiDataService)
		{
			_logger = logger;

			_apiDataService = apiDataService;
		}

		[HttpGet]
		public async Task<IActionResult> Get(string code, bool analog)
		{
			IEnumerable<StandardProductModel> result;
			
			try
			{
				result = await _apiDataService.GetDataMultipleSourceAsync(code, analog);
			}
			catch (Exception ex)
			{
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
					ExpressDelivery = products.MinBy(x => x.DeliveryMin),
					Optimal = myLogic.GetOptimalProduct(products)
				});

		}
	}
}
