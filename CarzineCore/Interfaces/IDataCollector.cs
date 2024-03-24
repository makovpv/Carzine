using CarzineCore.Models;

namespace CarzineCore.Interfaces
{
	public interface IDataCollector
	{
		public Task<List<StandardProductModel>> GetCalculatedDataAsync(string detailCode, bool includeAnalogs);

		public StandardProductModel GetProductByHash(int hash);
	}
}
