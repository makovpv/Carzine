using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carzine.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TranslationController : ControllerBase
	{
		private readonly ILogger<TranslationController> _logger;

		private readonly IDataTranslatorService _translationService;

		public TranslationController(ILogger<TranslationController> logger, IDataTranslatorService translationService)
		{
			_logger = logger;

			_translationService = translationService;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IEnumerable<TranslationDto> GetAllTranslation()
		{
			return _translationService.GetAllTranslations();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> AddTranslation([FromBody] TranslationDto translation)
		{
			try
			{
				await _translationService.AddTranslationAsync(translation.enName, translation.ruName);

				return StatusCode(StatusCodes.Status200OK);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{key}")]
		public async Task<IActionResult> DeleteTranslationAsync(string key)
		{
			try
			{
				await _translationService.DeleteTranslationAsync(key);

				return StatusCode(StatusCodes.Status200OK);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
