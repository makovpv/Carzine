using Carzine.Models;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IDataCollector _dataCollector;

		public CartController(IOrderRepository orderRepository, IDataCollector dataCollector)
		{
			_orderRepository = orderRepository;
			_dataCollector = dataCollector;
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddAsync([FromBody] UserCartItemViewModel item)
		{
			var product = _dataCollector.GetProductByHash(item.code); // by hash
			
			var id = await _orderRepository.AddToCartAsync(
				User.Identity.IsAuthenticated ? User.Identity.Name : item.uid,
				item.code, 
				product
			);

			return Ok(id);
		}

		[Authorize]
		[HttpPost("order")]
		public async Task<IActionResult> OrderAsync()
		{
			var id = await _orderRepository.MakeOrderFromCartAsync(User.Identity.Name);
			
			return Ok(id);
		}

		[HttpPost("remove")]
		public async Task<IActionResult> DeleteAsync([FromBody] UserCartItemViewModel item)
		{
			await _orderRepository.RemoveFromCartAsync(
				item.code,
				User.Identity.IsAuthenticated ? User.Identity.Name : item.uid
			);

			return Ok();
		}

		[HttpGet("{uid}")]
		public async Task<IActionResult> GetUserCartAsync(string uid)
		{
			var res = await _orderRepository.GetCartAsync(User.Identity.IsAuthenticated ? User.Identity.Name : uid);

			return Ok(res);
		}

		[Authorize]
		[HttpPost("merge/{uid}")]
		public async Task<IActionResult> MergeAsync(string uid)
		{
			var res = await _orderRepository.MergeUserCartAsync(uid, User.Identity.Name);
			
			return Ok(res);
		}
	}
}
