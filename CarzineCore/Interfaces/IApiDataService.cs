﻿using CarzineCore.Models;

namespace CarzineCore.Interfaces
{
	public interface IApiDataService
	{
		public List<StandardProductModel> GetProducts();

		public Task<IEnumerable<StandardProductModel>> GetDataMultipleSourceAsync(string detailCode, bool includeAnalogs);

		public Task<string> CreateApecOrderAsync();
		
		public Task<string> CreateApmOrderAsync(PreOrderModel preOrder);

		public Task<AcatSearchResult> SearchByVinAsync(string vin);

		public Task<AcatGroupResult?> GetAcatGroupsAsync(AcatGroupInfo groupInfo);

		public Task<AcatPartsSearchResult> GetAcatPartsAsync(AcatGroupInfo groupInfo);

		public Task<Stream> GetAcatImageSourceAsync(AcatGroupInfo groupInfo);
	}
}
