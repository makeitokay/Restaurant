using AuthService.Controllers.Dto.Registration;
using FluentValidation;
using Model.Entities;

namespace AuthService.Validators;

public class RegistrationRequestDtoValidator : AbstractValidator<RegistrationRequestDto>
{
	public RegistrationRequestDtoValidator()
	{
		RuleFor(dto => dto.Email)
			.EmailAddress();

		RuleFor(dto => dto.Password)
			.MinimumLength(8);

		RuleFor(dto => dto.Role)
			.IsEnumName(typeof(Role));
	}
}