using CarzineCore.Models;

namespace CarzineCore.Interfaces
{
	public interface IDbDataService
	{
		public Task<int> AddPreOrderAsync(PreOrderModel preorder);

		public Task<IEnumerable<PreOrderDto>> GetPreOrdersAsync();

		public Task<PreOrderDto> GetPreOrderAsync(int preOrderId);
	}
}
