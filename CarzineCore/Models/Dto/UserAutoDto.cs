using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public record UserAutoDto(string Vin, string markName, string modelName, int year)
	{
		public UserAutoDto() : this(default, default, default, default) { }
	}
}
