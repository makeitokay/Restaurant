using Model.Entities;

namespace AuthService.Controllers.Dto.Registration;

public class RegistrationRequestDto
{
	public string Username { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Password { get; set; } = default!;

	public string Role { get; set; } = default!;
}