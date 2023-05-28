using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("OrderDish")]
public class OrderDish : BaseEntity
{
	[Column("OrderId")]
	public int OrderId { get; set; }

	public virtual Order Order { get; set; } = default!;
	
	[Column("DishId")]
	public int DishId { get; set; }

	public virtual Dish Dish { get; set; } = default!;
	
	[Column("Quantity")]
	public int Quantity { get; set; }
	
	[Column("Price")]
	public decimal Price { get; set; }
}