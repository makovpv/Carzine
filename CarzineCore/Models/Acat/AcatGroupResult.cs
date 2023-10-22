using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Models
{
	public class AcatGroupResult
	{
		public AcatType type { get; set; }

		public AcatMark Mark { get; set; }

		public AcatModelModel Model { get; set; }

		public AcatModificationModel Modification { get; set; }

		public AcatGroupModel? Group { get; set; }

		public AcatGroupModel[] Groups { get; set; }

		public string? Criteria { get; set; }
	}

	public class AcatType
	{
		public string Name { get; set; }
		public string Id { get; set; }
	}

	public class AcatTransmission
	{
		public string name { get; set; }
		public string type { get; set; }
	}

	public class AcatMark
	{
		public string name { get; set; }
		public string image { get; set; }
		public bool archival { get; set; }
		public bool engine { get; set; }
		public bool vin { get; set; }
		public string id { get; set; }
		public bool hasModifications { get; set; }
		public bool searchParts { get; set; }
	}

	public class AcatModelModel
	{
		public string id { get; set; }
		public string name { get; set; }
		public string? modification { get; set; }
		public bool archival { get; set; }
		public string image { get; set; }
		public string? relevance { get; set; }

		public string? years { get; set; }
		public bool hasModifications { get; set; }
		public bool displayGroupsAsTree { get; set; }
	}

	public class AcatModificationModel
	{
		public string id { get; set; }

		public string name { get; set; }

		public string description { get; set; }
		public string modelId { get; set; }
		public string modelName { get; set; }
		public string modelImg { get; set; }
		public string catalogId { get; set; }
		public AcatVinParameterModel[] parameters { get; set; }

		public string vin { get; set; }
		public string frame { get; set; }
		public string criteria { get; set; }
		public string brand { get; set; }
		public string bodyType { get; set; }
		public string yeaar { get; set; }
		public string engine { get; set; }
		public string fuel { get; set; }
		public string region { get; set; }
		public string steering { get; set; }
		public string steeringId { get; set; }
		public AcatTransmission transmission { get; set; }
	}



	public class AcatGroupModel
	{
		public string id { get; set; }
		public bool hasSubgroups { get; set; }
		public bool hasParts { get; set; }

		public string name { get; set; }
		public string? description { get; set; }
		public string? parentId { get; set; }
		public string image { get; set; }

		public string? parentFullName { get; set; }
		public bool needLoadSubGroups { get; set; }

		public string[] subGroups { get; set; }
	}
}
