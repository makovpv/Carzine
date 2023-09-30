using Newtonsoft.Json;

namespace CarzineCore.Models
{
	public class ApmRootSearchResult
	{
		[JsonProperty("root")]
		public ApmSearchResult RootElement { get; set; }
	}

	public class ApmSearchResult
	{
		[JsonProperty("mainProducts")]
		public List<ApmProduct> mainProducts { get; set; }

		[JsonProperty("analogProducts")]
		public List<ApmProduct> analogProducts { get; set; }
	}

	public class ApmProduct
	{
		public int Id { get; set; }

		public string? code { get; set; }
		public string? name { get; set; }

		public decimal price { get; set; }

		[JsonProperty("currency_id")]
		public int currencyId { get; set; }

		public int available { get; set; }

		[JsonProperty("min_lot")]
		public int minLot { get; set; }

		public string make { get; set; }

		public decimal weight { get; set; }

		public string delivery { get; set; }

		public string priceName { get; set; }

		public int supplier_id { get; set; }

		[JsonProperty("price_info")]
		public decimal priceInfo { get; set; }

		[JsonProperty("currency_user")]
		public string CurrencyUser { get; set; }
	}
}
