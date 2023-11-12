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

			var usdRate = await DataCollector.GetCbrCursAsync("USD");

			var products = CarzineCalculator.CalcPriceRub(result.ToList(), usdRate);

			products.Sort(delegate (StandardProductModel x, StandardProductModel y) {
				if (x.Price == y.Price)
					return 0;
				
				return x.Price > y.Price ? 1 : -1;
			});

			return StatusCode(
				StatusCodes.Status200OK,
				new SearchResultViewModel()
				{
					Products = products.FillEmptyNames(),
					BestPrice = products.MinBy(x => x.PriceRub),
					ExpressDelivery = products.MinBy(x => x.DeliveryMin),
					Optimal = CarzineCalculator.GetOptimalProduct(products),
					UsdRate = usdRate
				});

		}

		[HttpGet("searchVin")]
		public async Task<IActionResult> SearchByVIN(string vin, bool requestEcoMode = false)
		{
			var groupInfo = new AcatGroupInfo
			{
				GroupType = "CARS_FOREIGN",
				Mark = "ford",
				Modification = "0417effa2c41f9665976e0ad9467387e",
				Model = "5c2447bd0d8d57b0bcbf7d8cc8407f3f",
				Group = string.Empty
			};

			if (!requestEcoMode)
			{
				var searchResult = await _apiDataService.SearchByVinAsync(vin);

				var acatVin = searchResult.vins.FirstOrDefault();

				groupInfo = new AcatGroupInfo
				{
					GroupType = acatVin?.type,
					Mark = acatVin?.mark,
					Modification = acatVin?.modification,
					Model = acatVin?.model,
					Group = string.Empty
				};
			}

			var acatGroups = await _apiDataService.GetAcatGroupsAsync(groupInfo);

			//var result = new AcatPartsSearchResult();

			//foreach (var group in acatGroups.Groups)
			//{
			//	groupInfo.ParentGroup = group.id;

			//	if (group.hasSubgroups)
			//	{
			//		var qqq = await _apiDataService.GetAcatGroupsAsync(groupInfo);
			//	}

			//	if (group.hasParts)
			//	{
			//		result = await _apiDataService.GetAcatPartsAsync(groupInfo);
			//	}
			//}

			return StatusCode(StatusCodes.Status200OK, acatGroups);
		}

		[HttpGet("groups")]
		public async Task<IActionResult> GetGroups(string GroupType, 
			string Mark, 
			string Modification, 
			string Model,
			string Group, string? ParentGroup)
		{
			var groupInfo = new AcatGroupInfo()
			{
				Group = Group,
				Mark = Mark,
				GroupType = GroupType,
				Modification = Modification,
				Model = Model,
				ParentGroup = ParentGroup
			};

			var result = await _apiDataService.GetAcatGroupsAsync(groupInfo);

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[HttpGet("parts")]
		public async Task<IActionResult> GetParts(string GroupType,
			string Mark,
			string Modification,
			string Model,
			string Group, string? ParentGroup)
		{
			var groupInfo = new AcatGroupInfo()
			{
				Group = Group,
				Mark = Mark,
				GroupType = GroupType,
				Modification = Modification,
				Model = Model,
				ParentGroup = ParentGroup
			};

			var result = await _apiDataService.GetAcatPartsAsync(groupInfo);

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[HttpGet("scheme")]
		[Produces("image/png")]
		public async Task<IActionResult> GetScheme(string GroupType,
			string Mark,
			string Modification,
			string Model,
			string Group, string? ParentGroup)
		{
			var groupInfo = new AcatGroupInfo()
			{
				Group = Group,
				Mark = Mark,
				GroupType = GroupType,
				Modification = Modification,
				Model = Model,
				ParentGroup = ParentGroup
			};

			var imgSource = await _apiDataService.GetAcatImageSourceAsync(groupInfo);

			return File(imgSource, "image/png");
		}
	}
}
