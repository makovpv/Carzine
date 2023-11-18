using Newtonsoft.Json;

namespace CarzineCore.Models
{
	public class PreOrderModel
	{
		public int Id { get; set; }

		public string Phone { get; set; }

		public StandardProductModel Product { get; set; }

		public DateTime Date { get; set; }

		public ClientStatus ClientStatus { get; set; }

		public SupplyerStatus SupplyerStatus { get; set; }
	}
}
