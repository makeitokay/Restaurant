using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

public abstract class BaseAuditableEntity : BaseEntity
{
	[Column("CreatedAtUtc")]
	public DateTimeOffset CreatedAtUtc { get; set; } = default!;

	[Column("UpdatedAtUtc")]
	public DateTimeOffset UpdatedAtUtc { get; set; } = default!;
}