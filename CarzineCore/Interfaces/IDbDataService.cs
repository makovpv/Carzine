using CarzineCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore.Interfaces
{
	public interface IDbDataService
	{
		public Task<int> AddPreOrderAsync(PreOrderModel preorder);

		public Task<IEnumerable<PreOrderModel>> GetPreOrdersAsync();
	}
}
