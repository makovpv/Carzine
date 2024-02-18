using CarzineCore;
using CarzineCore.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;

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
		public Dictionary<string, string> GetAllTranslations()
		{
			var result = new Dictionary<string, string>
			{
				{ "en", "AAA" },
				{ "RU", "RRR" }
			};

			return result;
		}
	}
}
