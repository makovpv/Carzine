namespace CarzineCore.Interfaces
{
	public interface IMailService
	{
		public void SendEmail(string addressTo);
		public Task SendEmailAsync(string addressTo, string message);
	}
}
