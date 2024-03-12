namespace CarzineCore.Models
{
	public record UserAutoDto(string Vin, string markName, string modelName, int year)
	{
		public UserAutoDto() : this(default, default, default, default) { }
	}
}
