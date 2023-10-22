namespace CarzineCore.Models
{
	public class ProductModel
	{
		public decimal Price { get; set; }
		/// <summary>
		/// Product name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Manufacturer name
		/// </summary>
		public string Make { get; set; }
		/// <summary>
		/// Store Name
		/// </summary>
		public string PriceName { get; set; }

		/// <summary>
		/// Min items count to order
		/// </summary>
		public int MinOrderAmount { get; set; }

		/// <summary>
		/// Max items count to order
		/// </summary>
		public int MaxOrderAmount { get; set; }
	}

	public class StandardProductModel
	{
		public string Name { get; set; }

		public string PartNumber { get; set; }

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

		public ApiSource SourceId { get; set; }

		public bool? IsOriginal { get; set; }
	}
}
