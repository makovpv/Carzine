using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IDbDataService _dataService;
		private readonly ILogger<OrderController> _logger;


		public OrderController(IDbDataService dbService, ILogger<OrderController> logger)
		{
			_dataService = dbService;
			_logger = logger;
		}

		[HttpPost("preorder")]
		public async Task<IActionResult> AddPreOrderAsync([FromBody] PreOrderModel preorder)
		{
			var result = await _dataService.AddPreOrderAsync(preorder);
			
			return StatusCode(StatusCodes.Status201Created, result);
		}

		[HttpGet("preorder")]
		public async Task<IActionResult> GetPreOrdersAsync()
		{
			var result = await _dataService.GetPreOrdersAsync();

			return StatusCode(StatusCodes.Status201Created, result);
		}
	}
}
