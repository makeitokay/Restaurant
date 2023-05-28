using Model.Entities;

namespace AuthService.Controllers.Dto.Registration;

public class UserDto
{
	public string Email { get; set; } = default!;
	public string Username { get; set; } = default!;
	public string Role { get; set; } = default!;
}