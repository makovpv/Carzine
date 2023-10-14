namespace CarzineCore.Models
{
	public class ApecSearchResult
	{

	}

	public class ApecProduct
	{
		public string PartDescription { get; set; }
		public int DeliveryDays { get; set; }
		public int QtyInStock { get; set; }
		public int MinOrderQty { get; set; }
		public decimal Price { get; set; }
		public decimal WeightPhysical { get; set; }
		public decimal WeightVolume { get; set; }
		public string Brand { get; set; }
		public string PartNumber { get; set; }
		public int SupplierID { get; set; }
	}

	public class ApecDeliveryPoint
	{
		public int DeliveryPointID { get; set; }
		public string DeliveryPointName { get; set; }
	}

	public class ApecBrand
	{
		public string Brand { get; set; }

		public string Description { get; set; }
	}
}
