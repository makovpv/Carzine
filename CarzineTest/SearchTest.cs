using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarzineTest
{
	public class SearchTest
	{
		private FakeApiDataService _apiDataService;

		public SearchTest()
		{
			
		}

		[SetUp]
		public void Setup()
		{
			var myConfig = new Dictionary<string, string> { };

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(myConfig)
				.Build();

			_apiDataService = new FakeApiDataService(configuration, null, null);

		}

		[Test]
		public async Task CachingVinRequestTest()
		{
			_ = await _apiDataService.SearchByVinAsync("VIN01");
			_ = await _apiDataService.SearchByVinAsync("VIN02");
			_ = await _apiDataService.SearchByVinAsync("Vin01");

			Assert.That(_apiDataService.VinRequestCount, Is.EqualTo(2));
		}

		[Test]
		public async Task CachingGroupRequestTest()
		{
			_ = await _apiDataService.GetAcatGroupsAsync(new AcatGroupInfo()
			{
				Group = "",
				Modification = "modification-1"
			});

			_ = await _apiDataService.GetAcatGroupsAsync(new AcatGroupInfo()
			{
				Group = "",
				Modification = "modification-2"
			});

			_ = await _apiDataService.GetAcatGroupsAsync(new AcatGroupInfo()
			{
				Group = "",
				Modification = "modification-1"
			});

			Assert.That(_apiDataService.GroupRequestCount, Is.EqualTo(2));
		}
	}

	public class FakeApiDataService : ApiDataService
	{
		public int VinRequestCount { get; set; }
		public int GroupRequestCount { get; set; }

		public FakeApiDataService(IConfiguration config, ILogger<ApiDataService> logger, IDataTranslatorService dataTranslationService) : base(config, logger, dataTranslationService)
		{
		}

		protected override async Task<AcatSearchResult> SearchByVinInternalAsync(string vin)
		{
			VinRequestCount++;
			
			return new AcatSearchResult()
			{
				vins = new AcatVinModel[]
				{
					new AcatVinModel()
					{
						markName = $"mark for {vin}"
					}
				}
			};
		}

		protected override async Task<AcatGroupResult?> GetAcatGroupsInternalAsync(AcatGroupInfo groupInfo)
		{
			GroupRequestCount++;

			return new AcatGroupResult()
			{
				
			};
		}
	}
}
