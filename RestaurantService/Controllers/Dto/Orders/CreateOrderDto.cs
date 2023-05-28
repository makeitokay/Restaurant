namespace RestaurantService.Controllers.Dto.Orders;

public class CreateOrderDto
{
	public string SpecialRequests { get; set; } = default!;

	public IEnumerable<OrderDishDto> Dishes { get; set; } = default!;
}