using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IDbDataService _dataService;
		private readonly IApiDataService _apiService;
		private readonly ILogger<OrderController> _logger;

		public OrderController(IDbDataService dbService, IApiDataService apiService, ILogger<OrderController> logger)
		{
			_logger = logger;

			_dataService = dbService;
			_apiService = apiService;
		}

		[Authorize]
		[HttpPost("preorder")]
		public async Task<IActionResult> AddPreOrderAsync([FromBody] StandardProductModel product)
		{
			var result = await _dataService.AddPreOrderAsync(product, User.Identity.Name);
			
			return StatusCode(StatusCodes.Status201Created, result);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("order/{preOrderId}")]
		public async Task<IActionResult> CreateOrderAsync(int preOrderId)
		{
			var preOrder = (await _dataService.GetPreOrderAsync(preOrderId)).ToPreOrderModel();
			
			switch (preOrder.Product.SourceId)
			{
				case ApiSource.Apec:
					var result1 = await _apiService.CreateApecOrderAsync();
					return StatusCode(StatusCodes.Status201Created, result1);
				case ApiSource.Apm:
					var result2 = await _apiService.CreateApmOrderAsync(preOrder);
					return StatusCode(StatusCodes.Status200OK, result2);
				case ApiSource.Emex:
					return StatusCode(StatusCodes.Status501NotImplemented, "Emex");
				default:
					return StatusCode(StatusCodes.Status400BadRequest, "Api doesn't support order creating");
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("preorder")]
		public async Task<IActionResult> GetPreOrdersAsync()
		{
			var preOrders = await _dataService.GetPreOrdersAsync();

			var result = preOrders.Select(x => x.ToPreOrderModel());

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[Authorize]
		[HttpGet("user-preorders")]
		public async Task<IActionResult> GetUserPreOrdersAsync()
		{
			var preOrders = await _dataService.GetPreOrdersByUserAsync(User.Identity.Name);

			var result = preOrders.Select(x => x.ToUserPreOrderModel());

			return StatusCode(StatusCodes.Status200OK, result);
		}

		[HttpGet("suppliers")]
		public IActionResult GetSuppliers()
		{
			Dictionary<int, string> data = new()
			{
				{ (int)ApiSource.Apm, "APM" },
				{ (int)ApiSource.Apec, "Apec" },
				{ (int)ApiSource.Emex, "Emex" }
			};

			return StatusCode(StatusCodes.Status200OK, data);
		}
	}
}
