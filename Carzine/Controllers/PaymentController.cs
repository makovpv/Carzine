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
				var order = await _orderRepostory.GetOrderAsync(orderId);

				if (order.User_email != User.Identity.Name)
				{
					throw new Exception($"User {User.Identity.Name} doesn't have order with id = {orderId}");
				}

				var res = await _paymentService.PayAsync(
					new PaymentData {
						amount = Convert.ToInt32(order.Total_Sum * 100),
						orderId = orderId.ToString(), //Guid.NewGuid().ToString(),
						bankId = BankId.Alfa,
						sessionType = "oneStep",
						additionalProps = new AdditionalPaymentData
						{
							returnUrl = $"{Request.Scheme}://{Request.Host}/account"
						}
					},
					isTestMode: IsTestMode()
				);

				return Ok(res);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
			
		}

		private bool IsTestMode()
		{
			return Request.Host.Host == "localhost";
		}
		
		[HttpPost("check/{paymentOrderId}")]
		[Authorize]
		public async Task<IActionResult> CheckAndSetPaymerOrderIdAsync(string paymentOrderId)
		{
			try
			{
				var res = await _paymentService.GetOrderStatusAsync(paymentOrderId);

				await _orderRepostory.SetPaymentOrderIdAsync(
					orderId: Convert.ToInt32(IsTestMode() ? res.orderNumber.Replace("T", "") : res.orderNumber),
					paymentOrderId: paymentOrderId,
					paymentStatus: res.paymentAmountInfo.paymentState
				);

				return Ok(res);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
