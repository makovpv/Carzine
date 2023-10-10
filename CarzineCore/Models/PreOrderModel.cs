using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public class PreOrderModel
	{
		public string Phone { get; set; }

		public string PartNumber { get; set; }

		public string Manufacturer { get; set; }

		public decimal PriceRub { get; set; }

		public int? DeliveryMin { get; set; }
	}
}
