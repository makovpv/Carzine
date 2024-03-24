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
		private readonly IMailService _mailService;
		private readonly ILogger<UserController> _logger;

		public UserController(IDbUserService dbService, IMailService mailService, ILogger<UserController> logger)
		{
			_dataService = dbService;
			_mailService = mailService;
			_logger = logger;
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

			try
			{
				_logger.LogInformation($"new user {user.Name} has been registered");

				await _mailService.SendEmailAsync("makovpv@gmail.com", $"new user {user.Name} has been registered");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return StatusCode(StatusCodes.Status200OK);
		}
	}
}
