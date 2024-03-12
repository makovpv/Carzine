namespace CarzineCore.Models
{
	public enum RuleRangeType
	{
		price = 1,
		delivery = 2
	}
	
	public record RuleRangeDto(int? Id, int? Min, int? Max, decimal Value, RuleRangeType Type)
	{
		public RuleRangeDto() : this(default, default, default, default, default) { }
	}
}
