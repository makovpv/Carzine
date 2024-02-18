using CarzineCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore
{
	public class LanguageTranslatorService : IDataTranslatorService
	{
		private readonly IDbTranslationService _dbTranslationService;

		private Dictionary<string, string> _translations = new();
		//IMemoryCache ?

		public LanguageTranslatorService(IDbTranslationService dbTranslationService)
		{
			_dbTranslationService = dbTranslationService;

			Task.Run(() => StartPeriodicDictionaryRefreshAsync());
		}

		private async Task StartPeriodicDictionaryRefreshAsync()
		{
			using var pTimer = new PeriodicTimer(TimeSpan.FromMinutes(60));
			while (await pTimer.WaitForNextTickAsync())
			{
				_translations = _dbTranslationService.GetAllTranslations();
			}
		}

		public string Translate(string originalText)
		{
			if (!_translations.Any())
			{
				_translations = _dbTranslationService.GetAllTranslations();
			}

			foreach (var key in _translations.Keys)
			{
				var idx = originalText.IndexOf(key, StringComparison.InvariantCultureIgnoreCase);

				if (idx != -1)
				{
					return originalText.Replace(key, _translations[key], StringComparison.InvariantCultureIgnoreCase);
				}
			}

			return originalText;
		}
	}
}
