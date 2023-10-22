using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public class PreOrderDto
	{
		public int Id { get; set; }

		public string Phone { get; set; }

		public string PN { get; set; }

		public string Manufacturer { get; set; }

		public decimal PriceRub { get; set; }

		public int? DeliveryMin { get; set; }

		public int source_Id { get; set; }
	}
}
