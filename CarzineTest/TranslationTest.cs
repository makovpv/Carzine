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
		[TestCase("expansion tank", "расширительный бачок")]
		[TestCase("coolANt", "охлаждающая жидкость")]
		[TestCase("coolANt - COOLant", "coolANt - COOLant")]
		[TestCase("the best expansion tank of the world", "the best expansion tank of the world")]
		[TestCase("expansion tank, coolant", "расширительный бачок, охлаждающая жидкость")]
		[TestCase("expansion tank E327SR730I 94", "расширительный бачок E327SR730I 94")]
		public void TranslateTest(string original, string translated)
		{
			Assert.That(_translatorService.Translate(original), Is.EqualTo(translated));
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
				new TranslationDto("tank", "бак"),
				new TranslationDto("anti roll bar bush", "втулка стабилизатора поперечной устойчивости"),
				new TranslationDto("expansion tank", "расширительный бачок"),
				new TranslationDto("Coolant", "охлаждающая жидкость"),
				
			};

			return result;
		}
	}
}
