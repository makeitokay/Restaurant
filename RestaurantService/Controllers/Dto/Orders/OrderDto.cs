namespace RestaurantService.Controllers.Dto.Orders;

public class OrderDto
{
	public int OrderId { get; set; }

	public string Status { get; set; } = default!;
}