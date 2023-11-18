namespace CarzineCore.Models
{
	public class PreOrderDto
	{
		public int Id { get; set; }

		public string Phone { get; set; }

		public string PN { get; set; }

		public string Manufacturer { get; set; }

		public decimal Price_Rub { get; set; }
		
		public decimal Supplyer_price { get; set; }

		public int? Delivery_Min { get; set; }

		public int Source_Id { get; set; }

		public DateTime Date { get; set; }

		public decimal Delivery_cost { get; set; }

		public decimal Extra_charge { get; set; }

		public decimal Volume { get; set; }

		public decimal Weight { get; set; }

		public ClientStatus Client_status { get; set; }

		public SupplyerStatus Supplyer_status { get; set;}
	}
}
