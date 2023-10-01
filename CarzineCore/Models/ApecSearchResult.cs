namespace CarzineCore.Models
{
	public class ApecSearchResult
	{

	}

	public class ApecProduct
	{
		public string Name { get; set; }

		public string Brand { get; set; }
		
		public decimal Price { get; set; }

		public int Qty { get; set; }

		public int DeliveryDaysMin { get; set; }

		public int DeliveryDaysMax { get; set; }

		public decimal WeightPhysical { get; set; }

		public decimal WeightVolume { get; set; }
	}
}
