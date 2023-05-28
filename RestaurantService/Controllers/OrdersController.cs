using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using RestaurantService.Controllers.Dto.Orders;

namespace RestaurantService.Controllers;

[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
	private readonly IRepository<Order> _orderRepository;
	private readonly IRepository<Dish> _dishRepository;
	private readonly IUserRepository _userRepository;
	private readonly IRepository<OrderDish> _orderDishRepository;

	public OrdersController(
		IRepository<Order> orderRepository,
		IRepository<Dish> dishRepository,
		IUserRepository userRepository,
		IRepository<OrderDish> orderDishRepository)
	{
		_orderRepository = orderRepository;
		_dishRepository = dishRepository;
		_userRepository = userRepository;
		_orderDishRepository = orderDishRepository;
	}

	[HttpPost]
	public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderDto createOrderDto)
	{
		if (!createOrderDto.Dishes.Any())
		{
			return BadRequest("No dishes requested in order");
		}

		var user = await _userRepository.FindUserByEmailAsync(User.Claims.GetEmail());
		if (user is null)
		{
			return BadRequest("User is not found");
		}

		var order = new Order
		{
			SpecialRequests = createOrderDto.SpecialRequests,
			Status = OrderStatus.Queued,
			UserId = user.Id
		};

		var orderDishes = new List<OrderDish>();
		var updatedDishes = new List<Dish>();
		foreach (var dishDto in createOrderDto.Dishes)
		{
			var dish = await _dishRepository.FindAsync(dishDto.Id);
			if (dish is null)
			{
				return BadRequest($"Dish with id = `{dishDto.Id}` not found");
			}

			if (dish.Quantity < dishDto.Quantity)
			{
				return BadRequest($"Requested quantity is not available for dish with id = `{dishDto.Id}`.\n" +
				                  $"Available quantity is {dish.Quantity}");
			}

			orderDishes.Add(new OrderDish
			{
				DishId = dish.Id,
				Order = order,
				Price = dish.Price,
				Quantity = dishDto.Quantity
			});

			dish.Quantity -= dishDto.Quantity;
			updatedDishes.Add(dish);
		}

		var createdOrder = await _orderRepository.CreateAsync(order);
		await _orderDishRepository.CreateRangeAsync(orderDishes);
		await _dishRepository.UpdateRangeAsync(updatedDishes);

		return Ok(GetOrderDto(createdOrder));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetOrderAsync(int id)
	{
		var order = await _orderRepository.FindAsync(id);
		if (order is null)
		{
			return BadRequest("Order is not found");
		}

		return Ok(GetOrderDto(order));
	}

	private static OrderDto GetOrderDto(Order order)
	{
		return new OrderDto
		{
			OrderId = order.Id,
			Status = order.Status.ToString()
		};
	}
}