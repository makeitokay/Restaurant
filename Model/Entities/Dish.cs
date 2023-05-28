using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("Dish")]
public class Dish : BaseEntity
{
	[Column("Name")]
	public string Name { get; set; } = default!;

	[Column("Description")]
	public string Description { get; set; } = default!;

	[Column("Price")]
	public decimal Price { get; set; }
	
	[Column("Quantity")]
	public int Quantity { get; set; }
}