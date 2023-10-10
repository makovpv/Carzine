using CarzineCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Interfaces
{
	public interface IApiDataService
	{
		public List<StandardProductModel> GetProducts();

		public Task<IEnumerable<StandardProductModel>> GetDataSingleSourceAsync(string detailCode);

		public Task<IEnumerable<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode);

	}
}
