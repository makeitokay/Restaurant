namespace RestaurantService.Controllers.Dto.Dishes;

public class CreateDishDto
{
	public string Name { get; set; } = default!;

	public string Description { get; set; } = default!;
	
	public int Quantity { get; set; }
	public decimal Price { get; set; }
}