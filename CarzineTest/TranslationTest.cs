using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarzineTest
{
	public class TranslationTest
	{
		private IDataTranslatorService _translatorService;

		[SetUp]
		public void Setup()
		{
			_translatorService = new LanguageTranslatorService(new FakeDbTranslationService());
		}

		[Test]
		public void Test1()
		{
			Assert.That(_translatorService.Translate("ru"), Is.EqualTo("RRR"));
			Assert.That(_translatorService.Translate("fresh en news"), Is.EqualTo("fresh AAA news"));
		}
	}

	class FakeDbTranslationService : IDbTranslationService
	{
		public Task AddTranslationAsync(string key, string translation)
		{
			throw new System.NotImplementedException();
		}

		public Task DeleteTranslationAsync(string key)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync()
		{
			var result = new List<TranslationDto>
			{
				new TranslationDto("en", "AAA"),
				new TranslationDto("RU", "RRR")
			};

			return result;
		}
	}
}
