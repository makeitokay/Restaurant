using FluentValidation;
using RestaurantService.Controllers.Dto.Dishes;

namespace RestaurantService.Validators;

public class CreateDishDtoValidator : AbstractValidator<CreateDishDto>
{
	public CreateDishDtoValidator()
	{
		RuleFor(dto => dto.Quantity)
			.GreaterThan(0);

		RuleFor(dto => dto.Price)
			.GreaterThan(0);
	}
}