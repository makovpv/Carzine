using Carzine.Auth;
using CarzineCore;
using CarzineCore.Interfaces;
using CarzineCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Carzine.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IDbUserService _dataService;

		public UserController(IDbUserService dbService)
		{
			_dataService = dbService;
		}

		[HttpPost("token")]
		public async Task<IActionResult> GetToken([FromBody] Person loginData)
		{
			var userData = await _dataService.GetUserByName(loginData.Email);

			if (userData == null || userData.Pwd != loginData.Password)
			{
				return StatusCode(StatusCodes.Status401Unauthorized, "Wrong username or password");
			}

			var claims = new List<Claim> {
				new Claim(ClaimTypes.Name, loginData.Email)
			};

			if (userData.Is_Admin)
			{
				claims.Add(new Claim(ClaimTypes.Role, "Admin"));
			}

			var expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(120));

			var jwt = new JwtSecurityToken(
				issuer: AuthOptions.ISSUER,
				audience: AuthOptions.AUDIENCE,
				claims: claims,
				expires: expires,
				signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
			);

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var response = new
			{
				access_token = encodedJwt,
				userName = loginData.Email,
				expires = expires,
				isProfUser = userData.Is_Admin
			};

			return StatusCode(StatusCodes.Status200OK, Results.Json(response));
		}

		[HttpPost("signup")]
		public async Task<IActionResult> SignUpAsync([FromBody] UserDto user)
		{
			try
			{
				await _dataService.AddUserAsync(user.Name, user.Pwd, user.Phone);
			}
			catch (CarzineException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}

			return StatusCode(StatusCodes.Status200OK);
		}
	}
}
