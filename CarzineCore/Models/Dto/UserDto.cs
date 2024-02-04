using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public class UserDto
	{
		public string Name { get; set; }
		public string Pwd { get; set; }
		public string Phone { get; set; }
		public bool Is_Admin { get; set; }
	}
}
