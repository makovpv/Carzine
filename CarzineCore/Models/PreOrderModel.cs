﻿using Newtonsoft.Json;

namespace CarzineCore.Models
{
	public class PreOrderModel
	{
		public int Id { get; set; }

		public string Phone { get; set; }

		public string UserEmail { get; set; }

		public StandardProductModel Product { get; set; }

		public DateTime Date { get; set; }

		public int ClientStatus { get; set; }

		public SupplyerStatus SupplyerStatus { get; set; }

		public string PaymentOrderState { get; set; }
	}
}
