using CarzineCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<PaymentService> _logger;
		private readonly string _token;
		private readonly string _paymentUri;

		public PaymentService(IConfiguration config, ILogger<PaymentService> logger)
		{
			_logger = logger;
			_configuration = config;

			_token = config.GetSection("PaymentApi")["token"];
			_paymentUri = config.GetSection("PaymentApi")["url"];
		}

		public async Task<RegisterOrderResponse> PayAsync(PaymentData data, bool isTestMode)
		{
			using var httpClient = new HttpClient();

			var orderNumber = isTestMode ? $"T{data.orderId}" : data.orderId;

			var requestUri = $"{_paymentUri}register.do?token={_token}&orderNumber={orderNumber}&amount={data.amount}&returnUrl={data.additionalProps.returnUrl}";

			var response = await httpClient.PostAsync(requestUri, null);

			var responseBody = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<RegisterOrderResponse>(responseBody);
		}

		public async Task<OrderPaymentStatusModel> GetOrderStatusAsync(string orderId)
		{
			using var httpClient = new HttpClient();

			var requestUri = $"{_paymentUri}getOrderStatusExtended.do?token={_token}&orderId={orderId}";

			var response = await httpClient.PostAsync(requestUri, null);

			var responseBody = await response.Content.ReadAsStringAsync();

			var result = JsonConvert.DeserializeObject<OrderPaymentStatusModel>(responseBody);

			return result;
		}

		public async Task<string> DepositAsync(string orderId)
		{
			using var httpClient = new HttpClient();

			return null;
		}
	}

	public class RegisterOrderResponse
	{
		public string orderId { get; set; }
		public string formUrl { get; set; }
		public int errorCode { get; set; }
		public string errorMessage { get; set; }
	}

	public class PaymentData
	{
		public int amount { get; set; }
		public string orderId { get; set; }

		public BankId bankId { get; set; }
		public string sessionType { get; set; }

		public AdditionalPaymentData additionalProps { get; set; }
	}

	public class AdditionalPaymentData
	{
		public string returnUrl { get; set; }
	}

	public class OrderPaymentStatusModel
	{
		public string errorCode { get; set; }
		public string errorMessage { get; set; }
		public int orderStatus { get; set; }
		public string orderNumber { get; set; }
		public PaymentAmountInfo paymentAmountInfo { get; set; }
	}

	public class PaymentAmountInfo
	{
		public int approvedAmount { get; set; }
		public int depositAmount { get; set; }
		public int feeAmount { get; set; }
		public int totalAmount { get; set; }
		public string paymentState { get; set; }
	}

	public enum BankId
	{
		Alfa = 100,
		Tinkoff = 700
	}
}
