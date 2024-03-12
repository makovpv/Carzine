using Carzine.Models;
using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SearchController : ControllerBase
	{
		private readonly ILogger<SearchController> _logger;

		private readonly IApiDataService _apiDataService;
		private readonly IDbActionLogService _dbActionLogService;
		private readonly IDbDataRepository _dbDataRepository;

		public SearchController(ILogger<SearchController> logger, IApiDataService apiDataService, 
			IDbActionLogService dbActionLogService, IDbDataRepository dataRepository)
		{
			_logger = logger;
			_apiDataService = apiDataService;
			_dbActionLogService = dbActionLogService;
			_dbDataRepository = dataRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Get(string code, bool analog)
		{
			IEnumerable<StandardProductModel> productData;
			
			try
			{
				productData = await _apiDataService.GetDataMultipleSourceAsync(code, analog);
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex.Message);
				_logger.LogError(ex.StackTrace);
				
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}

			var usdRate = await DataCollector.GetCbrCursAsync("USD");

			var products = await new CarzineCalculator(_dbDataRepository).CalcProductComponentsAsync(productData, usdRate);

			products.Sort(delegate (StandardProductModel x, StandardProductModel y) {
				if (x.PriceRub == y.PriceRub)
					return 0;
				
				return x.PriceRub > y.PriceRub ? 1 : -1;
			});

			return StatusCode(
				StatusCodes.Status200OK,
				new SearchResultViewModel()
				{
					Products = products.FillEmptyNames(),
					BestPrice = CarzineCalculator.GetBestPriceProduct(products),
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

			AcatVinModel? acatVin = new();

			if (!requestEcoMode)
			{
				var searchResult = await _apiDataService.SearchByVinAsync(vin);

				if (searchResult.vins == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, "No vins in search result");
				}
				
				acatVin = searchResult.vins.FirstOrDefault();

				groupInfo = new AcatGroupInfo
				{
					GroupType = acatVin?.type,
					Mark = acatVin?.mark,
					Modification = acatVin?.modification,
					Model = acatVin?.model,
					Group = string.Empty
				};
			}

			if (User.Identity.IsAuthenticated && acatVin?.model != null)
			{
				try
				{
					_ = Task.Run(() => _dbActionLogService.LogUserAutoAsync(vin, acatVin, User.Identity.Name));
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message);
				}
			}

			var acatGroups = await _apiDataService.GetAcatGroupsAsync(groupInfo);

			return StatusCode(StatusCodes.Status200OK, acatGroups);
		}

		[Authorize]
		[HttpGet("garage/{count}")]
		public async Task<IActionResult> GetUserGarage(int count)
		{
			var cars = await _dbActionLogService.GetUserAutoAsync(User.Identity.Name, count);

			return Ok(cars);
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

			result.numbers = result.numbers.DistinctBy(x => x.Id + x.number + x.name).ToArray();

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[HttpGet("parts-by-name")]
		public async Task<IActionResult> GetPartsByName(string name)
		{
			var rootGroups = new string[] { "MvCfmoAxMDE3NjgzM_CfmoEx", "MvCfmoAxMDE3NjgzNPCfmoEx", "MvCfmoAxMDE3Njg1MvCfmoEw",
				"MvCfmoAxMDE3Njg1NPCfmoEw", "MvCfmoAxMDE3Njg1M_CfmoEw", "MvCfmoAxMDE3NjgzOPCfmoEx", "MvCfmoAxMDE3Njg0N_CfmoEx",
				"MvCfmoAxMDE3Njg0NPCfmoEx", "MvCfmoAxMDE3Njg0OPCfmoEx", "MvCfmoAxMDE3NjgzOfCfmoEx", "MvCfmoAxMDE3Njg0MPCfmoEx",
				"MvCfmoAxMDE3NjgzNfCfmoEx", "MvCfmoAxMDE3Njg0MvCfmoEx", "MvCfmoAxMDE3Njg0M_CfmoEx", "MvCfmoAxMDE3Njg0MfCfmoEx",
				"MvCfmoAxMDE3NjgzN_CfmoEx", "MvCfmoAxMDE3Njg0OfCfmoEx", "MvCfmoAxMDE3Njg1MfCfmoEx", "MvCfmoAxMDE3Njg0NvCfmoEx",
				"MvCfmoAxMDE3Njg1MPCfmoEx", "MvCfmoAxMDE3NjgzNvCfmoEx", "MvCfmoAxMDE3Njg0NfCfmoEx"
			};

			return StatusCode(StatusCodes.Status200OK, "not implemented yet");
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
