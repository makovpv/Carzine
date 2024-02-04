using CarzineCore.Models;

namespace CarzineCore.Interfaces
{
	public interface IDbDataService
	{
		public Task<int> AddPreOrderAsync(StandardProductModel product, string userName);

		public Task<IEnumerable<PreOrderDto>> GetPreOrdersAsync();

		public Task<PreOrderDto> GetPreOrderAsync(int preOrderId);

		public Task<IEnumerable<PreOrderDto>> GetPreOrdersByUserAsync(string userEmail);
	}

	public interface IDbUserService
	{
		public Task AddUserAsync(string userName, string pwd, string phone);

		public Task<UserDto?> GetUserByName(string userName);
	}
}
