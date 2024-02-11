namespace CarzineCore.Models
{
	public record StatusDto(int id, string name)
	{
		public StatusDto() : this(default, default) { }
	}
}
