using CarzineCore.Models;

namespace CarzineCore.Interfaces
{
	public interface IDbDataRepository
	{
		public Task<int> AddPreOrderAsync(StandardProductModel product, string userName);

		public Task<IEnumerable<OrderDto>> GetOrdersAsync();

		public Task<OrderDto> GetOrderAsync(int preOrderId);

		public Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(string userEmail);

		public Task SetOrderClientStatus(int orderId, ClientStatus status);

		public Task SetPaymentOrderIdAsync(int orderId, string paymentOrderId, string paymentStatus);

		public Task<IEnumerable<StatusDto>> GetClientStatusesAsync();

		public Task<IEnumerable<RuleRangeDto>> GetRuleRangesAsync();
		public Task<IEnumerable<RuleRangeDto>> GetRuleRangesAsync(RuleRangeType rangeType);

		public Task AddRuleRangeAsync(RuleRangeDto ruleRange);

		public Task DeleteRuleRangeAsync(int id);
	}

	public interface IOrderRepository
	{
		public Task<IEnumerable<CartItemDto>> GetCartAsync(string userName);
		public Task<int> AddToCartAsync(string userName, int hash, StandardProductModel product);
		public Task RemoveFromCartAsync(int id, string userName);
		public Task<int> MakeOrderFromCartAsync(string userName);
		public Task<int> MergeUserCartAsync(string uid, string userName);
	}

	public interface IDbUserService
	{
		public Task AddUserAsync(string userName, string pwd, string phone);

		public Task<UserDto?> GetUserByName(string userName);
	}

	public interface IDbTranslationService
	{
		public Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync();

		public Task AddTranslationAsync(string key, string translation);

		public Task DeleteTranslationAsync(string key);
	}

	public interface IDbActionLogService
	{
		public Task LogUserAutoAsync(string vin, AcatVinModel acatVin, string userName);

		public Task<IEnumerable<UserAutoDto>> GetUserAutoAsync(string userName, int limit);
	}
}
