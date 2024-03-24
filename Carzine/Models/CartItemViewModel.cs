namespace Carzine.Models
{
	public class CartItemViewModel
	{
		public int Code { get; set; }
		public string Name { get; set; }
		public string PartNumber { get; set; }
		public string Manufacturer { get; set; }
		public decimal PriceRub { get; set; }
		public int DeliveryMin { get; set; }
		public int DeliveryMax { get; set; }
		public bool? IsOriginal { get; set; }
	}

	public record UserCartItemViewModel(string uid, int code)
	{
		public UserCartItemViewModel(): this(default, default) { }
	}
}
