namespace CarzineCore.Models
{
	public class ApmTokenResponse
	{
		public string name { get; set; }
		
		public string token { get; set; }
	}

	public class ApecTokenResponse
	{
		public string access_token { get; set; }

		public string token_type { get; set; }

		public int expires_in { get; set; }

		public DateTime expireDate { get; set; }
	}
}
