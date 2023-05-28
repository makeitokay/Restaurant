using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

public abstract class BaseEntity
{
	[Column("Id")]
	public int Id { get; set; }
}