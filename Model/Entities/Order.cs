using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("Order")]
public class Order : BaseAuditableEntity
{
	[Column("UserId")]
	public int UserId { get; set; }

	public virtual User User { get; set; } = default!;

	[Column("Status")]
	public OrderStatus Status { get; set; }
	
	[Column("SpecialRequests")]
	public string SpecialRequests { get; set; } = default!;
}