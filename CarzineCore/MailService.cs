using CarzineCore.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CarzineCore
{
	public class MailService : IMailService
	{
		private readonly string _mailFrom;
		private readonly SmtpClient _smtpClient;

		public MailService(IConfiguration config)
		{
			var section = config.GetSection("mailSettings");

			_smtpClient = new SmtpClient(section["host"])
				{
					EnableSsl = true,
					Port = Convert.ToInt16(section["port"]),
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(section["userName"], section["password"])
				};

			_mailFrom = section["userName"];
		}
		
		public void SendEmail(string addressTo)
		{
			try
			{
				_smtpClient.Send(new MailMessage(_mailFrom, addressTo, "test subject", "mail body"));
			}
			catch (Exception ex)
			{

			}
		}

		public async Task SendEmailAsync(string addressTo, string message)
		{
			await _smtpClient.SendMailAsync(new MailMessage(
				_mailFrom,
				addressTo,
				"test subject",
				message
				));
		}
	}
}
