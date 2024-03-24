namespace CarzineCore.Models
{
	public class StandardProductModel
	{
		public string Name { get; set; }

		public string PartNumber { get; set; }

		public ItemCodeType PartNumberType { get; set; }

		public string Manufacturer { get; set; }

		/// <summary>
		/// Price in USD
		/// </summary>
		public decimal Price { get; set; }
		public decimal PriceRub { get; set; }

		public int MinOrderAmount { get; set; }

		public int MaxOrderAmount { get; set; }

		public decimal Weight { get; set; }

		public decimal Volume { get; set; }

		public int DeliveryMin { get; set; }

		public int DeliveryMax { get; set; }
		public int DeliveryMinOriginal { get; set; }

		public int DeliveryMaxOriginal { get; set; }

		public ApiSource SourceId { get; set; }

		public bool? IsOriginal { get; set; }

		public decimal DeliveryCost { get; set; }

		public decimal ExtraCharge { get; set; }
	}
}
