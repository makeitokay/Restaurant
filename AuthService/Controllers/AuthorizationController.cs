using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Config;
using AuthService.Controllers.Dto.Registration;
using FluentValidation;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;

namespace AuthService.Controllers;

[Route("api/auth")]
public class AuthorizationController : ControllerBase
{
	private readonly IValidator<RegistrationRequestDto> _registrationRequestValidator;
	private readonly IPasswordManager _passwordManager;
	private readonly IUserRepository _userRepository;
	private readonly AuthOptions _authOptions;

	public AuthorizationController(
		IValidator<RegistrationRequestDto> registrationRequestValidator,
		IPasswordManager passwordManager,
		IUserRepository userRepository,
		IOptions<AuthOptions> authOptions)
	{
		_registrationRequestValidator = registrationRequestValidator;
		_passwordManager = passwordManager;
		_userRepository = userRepository;
		_authOptions = authOptions.Value;
	}

	[HttpPost("signup")]
	public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequestDto requestDto)
	{
		var validationResult = await _registrationRequestValidator.ValidateAsync(requestDto);
		if (!validationResult.IsValid)
		{
			return BadRequest(validationResult.ToString());
		}

		if (await _userRepository.FindUserByEmailAsync(requestDto.Email) is not null)
		{
			return BadRequest("User with specified email already exists");
		}

		var (passwordHash, passwordSalt) = _passwordManager.GetPasswordHash(requestDto.Password);

		var user = new User
		{
			Username = requestDto.Username,
			Email = requestDto.Email,
			Role = Enum.Parse<Role>(requestDto.Role),
			PasswordHash = passwordHash,
			PasswordSalt = passwordSalt
		};

		await _userRepository.CreateAsync(user);

		return Ok("Successful registration");
	}

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponseDto>> LoginAsync([FromBody] LoginRequestDto requestDto)
	{
		var user = await _userRepository
			.Items
			.SingleOrDefaultAsync(u => u.Email == requestDto.Email);

		if (user is null || !_passwordManager.VerifyPassword(requestDto.Password, user.PasswordHash, user.PasswordSalt))
		{
			return Unauthorized("Incorrect login or password");
		}
		
		var userClaims = new List<Claim>
		{
			new(ClaimTypes.Email, user.Email),
			new(ClaimTypes.Role, user.Role.ToString()),
		};

		var jwt = new JwtSecurityToken(
			issuer: _authOptions.JwtIssuer,
			audience: _authOptions.JwtAudience,
			claims: userClaims,
			expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
			signingCredentials: new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.JwtSecretKey)),
				SecurityAlgorithms.HmacSha256));

		return new LoginResponseDto { AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt) };
	}
}