using CarzineCore.Interfaces;
using CarzineCore.Models;
using System.Text.RegularExpressions;

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

			Task.Run(() => LoadTranslationsAsync()).Wait();
		}

		private async Task LoadTranslationsAsync()
		{
			_translations = (await _dbTranslationService.GetAllTranslationsAsync())
					.ToDictionary(x => x.enName.ToLower(), x => x.ruName);
		}

		private async Task StartPeriodicDictionaryRefreshAsync()
		{
			using var pTimer = new PeriodicTimer(TimeSpan.FromMinutes(60));
			while (await pTimer.WaitForNextTickAsync())
			{
				_translations = (await _dbTranslationService.GetAllTranslationsAsync())
					.ToDictionary(x => x.enName, x => x.ruName);
			}
		}

		public string Translate(string originalText)
		{
			var result = string.Empty;

			var tokens = originalText.Split(", ");

			//StringBuilder

			foreach (var token in tokens)
			{
				if (!Regex.Match(token, "[0-9]").Success)
				{
					result += (_translations.GetValueOrDefault(token.ToLower()) ?? token) + ", ";

					continue;
				}

				var digitWordIdx = Regex.Match(token, "[0-9]").Index;

				var tokenWithoutDigits = token[..(digitWordIdx - 2)];

				var translation = string.Concat(
					_translations.GetValueOrDefault(tokenWithoutDigits.ToLower()) ?? tokenWithoutDigits,
					token.AsSpan(digitWordIdx - 2));

				result += translation + ", ";
			}

			result = result.Remove(result.Length - 2);

			return result;
		}

		public async Task AddTranslationAsync(string key, string translation)
		{
			_translations.Add(key.ToLower(), translation);

			await _dbTranslationService.AddTranslationAsync(key, translation);
		}

		public async Task DeleteTranslationAsync(string key)
		{
			_translations.Remove(key.ToLower());

			await _dbTranslationService.DeleteTranslationAsync(key);
		}

		public IEnumerable<TranslationDto> GetAllTranslations()
		{
			return _translations.Select(x => new TranslationDto(x.Key, x.Value));
		}
	}
}
