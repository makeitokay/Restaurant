using AuthService.Controllers.Dto.Registration;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/users")]
public class UsersController : ControllerBase
{
	private readonly IUserRepository _userRepository;

	public UsersController(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	[HttpGet("me")]
	[Authorize]
	public async Task<ActionResult<UserDto>> GetMeAsync()
	{
		var user = (await _userRepository.FindUserByEmailAsync(User.Claims.GetEmail()))!;
		return new UserDto
		{
			Email = user.Email,
			Username = user.Username,
			Role = user.Role.ToString()
		};
	}
}