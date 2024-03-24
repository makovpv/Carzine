using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IDbDataRepository _dataRepository;
		private readonly IApiDataService _apiService;
		private readonly ILogger<OrderController> _logger;

		public OrderController(IDbDataRepository dbService, IApiDataService apiService, ILogger<OrderController> logger)
		{
			_logger = logger;

			_dataRepository = dbService;
			_apiService = apiService;
		}


		[Authorize(Roles = "Admin")]
		[HttpPost("status/{orderId}")]
		public async Task<IActionResult> SetOrderStatusAsync(int orderId, [FromBody] int statusId)
		{
			await _dataRepository.SetOrderClientStatus(orderId, (ClientStatus)statusId);

			return Ok();
		}

		//[Authorize(Roles = "Admin")]
		//[HttpPost("order/{preOrderId}")]
		//public async Task<IActionResult> CreateOrderAsync(int preOrderId)
		//{
		//	var preOrder = (await _dataRepository.GetOrderAsync(preOrderId)).ToPreOrderModel();
			
		//	switch (preOrder.Product.SourceId)
		//	{
		//		case ApiSource.Apec:
		//			var result1 = await _apiService.CreateApecOrderAsync();
		//			return StatusCode(StatusCodes.Status201Created, result1);
		//		case ApiSource.Apm:
		//			var result2 = await _apiService.CreateApmOrderAsync(preOrder);
		//			return StatusCode(StatusCodes.Status200OK, result2);
		//		case ApiSource.Emex:
		//			return StatusCode(StatusCodes.Status501NotImplemented, "Emex");
		//		default:
		//			return StatusCode(StatusCodes.Status400BadRequest, "Api doesn't support order creating");
		//	}
		//}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetOrdersAsync()
		{
			var orders = await _dataRepository.GetOrdersAsync();

			return StatusCode(StatusCodes.Status200OK, orders);
		}

		[Authorize]
		[HttpGet("own")]
		public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync()
		{
			var orders = await _dataRepository.GetOrdersByUserAsync(User.Identity.Name);

			return orders;
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

		[HttpGet("status")]
		public async Task<IActionResult> GetAllOrderClientStatuses()
		{
			var statuses = await _dataRepository.GetClientStatusesAsync();

			return StatusCode(StatusCodes.Status200OK, statuses);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("rules")]
		public async Task<IActionResult> GetAllRuleRangesAsync()
		{
			var rules = await _dataRepository.GetRuleRangesAsync();

			return Ok(rules);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("rule")]
		public async Task<IActionResult> AddRuleRangeAsync([FromBody] RuleRangeDto rule)
		{
			await _dataRepository.AddRuleRangeAsync(rule);

			return Ok();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("rule/{id}")]
		public async Task<IActionResult> DeleteRuleRangeAsync(int id)
		{
			try
			{
				await _dataRepository.DeleteRuleRangeAsync(id);

				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Что-то пошло не так");
			}
			
		}

	}
}
