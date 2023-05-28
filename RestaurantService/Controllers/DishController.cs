using FluentValidation;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using RestaurantService.Controllers.Dto.Dishes;

namespace RestaurantService.Controllers;

[Route("api/dishes")]
[Authorize(Roles = "Manager")]
public class DishController : ControllerBase
{
	private readonly IRepository<Dish> _dishRepository;
	private readonly IValidator<CreateDishDto> _dishDtoValidator;

	public DishController(
		IRepository<Dish> dishRepository,
		IValidator<CreateDishDto> dishDtoValidator)
	{
		_dishRepository = dishRepository;
		_dishDtoValidator = dishDtoValidator;
	}

	[HttpPost]
	public async Task<IActionResult> CreateDishAsync([FromBody] CreateDishDto createDishDto)
	{
		var validationResult = await _dishDtoValidator.ValidateAsync(createDishDto);
		if (!validationResult.IsValid)
		{
			return BadRequest(validationResult.ToString());
		}
		
		var dish = new Dish
		{
			Name = createDishDto.Name,
			Description = createDishDto.Description,
			Price = createDishDto.Price,
			Quantity = createDishDto.Quantity
		};

		var createdDish = await _dishRepository.CreateAsync(dish);

		return Ok(GetDishDto(createdDish));
	}

	[HttpPost("{id:int}")]
	public async Task<IActionResult> UpdateDishAsync(int id, [FromBody] CreateDishDto updateDishDto)
	{
		var dish = await _dishRepository.FindAsync(id);
		if (dish is null)
		{
			return BadRequest("Dish is not found");
		}

		var validationResult = await _dishDtoValidator.ValidateAsync(updateDishDto);
		if (!validationResult.IsValid)
		{
			return BadRequest(validationResult.ToString());
		}

		dish.Name = updateDishDto.Name;
		dish.Description = updateDishDto.Description;
		dish.Price = updateDishDto.Price;
		dish.Quantity = updateDishDto.Quantity;

		await _dishRepository.UpdateAsync(dish);

		return Ok(GetDishDto(dish));
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteDishAsync(int id)
	{
		var dish = await _dishRepository.FindAsync(id);
		if (dish is null)
		{
			return BadRequest("Dish is not found");
		}

		await _dishRepository.DeleteAsync(dish);

		return Ok("Dish successfully deleted");
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetDishAsync(int id)
	{
		var dish = await _dishRepository.FindAsync(id);
		if (dish is null)
		{
			return BadRequest("Dish is not found");
		}

		return Ok(GetDishDto(dish));
	}

	[HttpGet("menu")]
	public async Task<IActionResult> GetMenuAsync()
	{
		var dishes = await _dishRepository
			.Items
			.Where(d => d.Quantity > 0)
			.ToListAsync();

		return Ok(dishes.Select(GetDishDto));
	}

	private static DishDto GetDishDto(Dish dish)
	{
		return new DishDto
		{
			Id = dish.Id,
			Name = dish.Name,
			Price = dish.Price,
			Quantity = dish.Quantity,
			Description = dish.Description
		};
	}
}