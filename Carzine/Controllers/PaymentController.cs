using CarzineCore;
using CarzineCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly ILogger _logger;
		private readonly IPaymentService _paymentService;
		private readonly IDbDataRepository _orderRepostory;

		public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService, IDbDataRepository orderRepostory)
		{
			_logger = logger;
			_paymentService = paymentService;
			_orderRepostory = orderRepostory;
		}
		
		[HttpPost("callback")]
		public IActionResult Callback([FromBody] Object data)
		{
			_logger.LogInformation("in callback");
			
			return Ok();
		}

		[HttpPost("pay/{orderId}")]
		[Authorize]
		public async Task<IActionResult> PayAsync(int orderId)
		{
			try
			{
				var order = await _orderRepostory.GetPreOrderAsync(orderId);

				//var uName = User.Identity.Name;
				//var ff = order.User_email;

				var res = await _paymentService.PayAsync(new PaymentData
				{
					amount = Convert.ToInt32(order.Price_Rub * 100),
					orderId = orderId.ToString(), //Guid.NewGuid().ToString(),
					bankId = BankId.Alfa,
					sessionType = "oneStep",
					additionalProps = new AdditionalPaymentData
					{
						returnUrl = $"{Request.Scheme}://{Request.Host}/account"
					}
				});

				return Ok(res);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
			
		}
		
		[HttpPost("check/{orderId}")]
		[Authorize]
		public async Task<IActionResult> CheckAsync(string orderId)
		{
			try
			{
				var res = await _paymentService.GetOrderStatusAsync(orderId);

				return Ok(res);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
