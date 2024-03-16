using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Interfaces
{
	public interface IPaymentService
	{
		public Task<RegisterOrderResponse> PayAsync(PaymentData data);

		public Task<OrderPaymentStatusModel> GetOrderStatusAsync(string orderId);

		public Task<string> DepositAsync(string orderId);
	}
}
