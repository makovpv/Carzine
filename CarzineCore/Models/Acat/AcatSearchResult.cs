using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public class AcatSearchResult
	{
		public AcatVinModel[] vins { get; set; }
		public string[] marks { get; set; }
		//public List<string> marks { get; set; }
	}

	public class AcatVinModel
	{
		public string criteria { get; set; }
		public string modelName { get; set; }
		public string description { get; set; }
		
		public string[] optionCodes { get; set; }
		public AcatVinParameterModel[] parameters { get; set; }

		public string type { get; set; }
		public string mark { get; set; }

		public string markName { get; set; }

		public string model { get; set; }

		public string modification { get; set; }

		public string criteriaURI { get; set; }

		public string image { get; set; }

	}

	public class AcatVinParameterModel
	{
		public string key { get; set; }
		public string name { get; set; }

		public string value { get; set; }

		public int sortOrder { get; set; }
	}

}
