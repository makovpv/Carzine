using Newtonsoft.Json;

namespace CarzineCore.Models
{
	public class OrderDto
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Phone { get; set; }
		[JsonProperty("userEmail")]
		public string User_email { get; set; }
		public string Payment_Order_State { get; set; }
		[JsonProperty("TotalSum")]
		public decimal Total_Sum { get; set; }
		public int client_status_id { get; set; }
	}

	//public class PreOrderDto
	//{
	//	public int Id { get; set; }

	//	public string Phone { get; set; }

	//	public string User_email { get; set; }

	//	public string PN { get; set; }

	//	public string Manufacturer { get; set; }

	//	public decimal Price_Rub { get; set; }
		
	//	public decimal Supplyer_price { get; set; }

	//	public int? Delivery_Min { get; set; }

	//	public int Source_Id { get; set; }

	//	public DateTime Date { get; set; }

	//	public decimal Delivery_cost { get; set; }

	//	public decimal Extra_charge { get; set; }

	//	public decimal Volume { get; set; }

	//	public decimal Weight { get; set; }

	//	public int Client_status { get; set; }

	//	public SupplyerStatus Supplyer_status { get; set;}

	//	public string Payment_Order_State { get; set; }
	//}

	public enum ItemCodeType
	{
		PN = 1,
		VendorCode = 2 // Артикул
	}

	public record CartItemDto(
		int id,
		string item_code,
		ItemCodeType code_type,
		string name,
		int amount,
		decimal price_rub,
		int delivery_min
		)
	{
		public CartItemDto() : this(default, default, default, default, default, default, default) { }
	}
}
