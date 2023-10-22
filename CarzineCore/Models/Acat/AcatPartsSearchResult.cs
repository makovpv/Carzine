using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public class AcatPartsSearchResult
	{
		public AcatType Type { get; set; }

		public AcatMark Mark { get; set; }

		public AcatModelModel Model { get; set; }

		public AcatModificationModel Modification { get; set; }

		public AcatGroupModel group { get; set; }

		public AcatGroupModel? prev { get; set; }

		public AcatGroupModel? next { get; set; }

		public string image { get; set; }

		public AcatLabel[] labels { get; set; }

		public AcatNumber[] numbers { get; set; }
	}

	public class AcatNumber
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		public string number { get; set; }

		public string name { get; set; }

		public string description { get; set; }

		public string labelId { get; set; }

		public string groupId { get; set; }

	}

	public class AcatLabel
	{
		public AcatRect coordinate { get; set; }
		public string id { get; set; }
		public string name { get; set; }
	}

	public class AcatRect
	{
		public AcatPoint top { get; set; }
		public AcatPoint bottom { get; set; }
		public int width { get; set; }
		public int height { get; set; }

	}

	public class AcatPoint
	{
		public int x { get; set; }
		public int y { get; set; }

	}
}

