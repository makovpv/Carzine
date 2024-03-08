using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Carzine.Auth
{
	public class AuthOptions
	{
		public const string ISSUER = "CrzAuthServer";
		public const string AUDIENCE = "CrzAuthClient";
		const string KEY = "******";
		public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
			new(Encoding.UTF8.GetBytes(KEY));
	}

	public record class Person(string Email, string Password);
}
