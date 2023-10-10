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
		
		public OrderController(IDbDataService dbService)
		{
			_dataService = dbService;
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
