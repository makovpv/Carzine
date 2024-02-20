namespace CarzineCore.Models
{
	public record TranslationDto(string enName, string ruName)
	{
		public TranslationDto() : this(default, default) { }
	}
}
