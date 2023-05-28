using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("Users")]
public class User : BaseAuditableEntity
{
	[Column("Username")]
	public string Username { get; set; } = default!;

	[Column("Email")]
	public string Email { get; set; } = default!;

	[Column("PasswordHash")]
	public string PasswordHash { get; set; } = default!;
	
	[Column("PasswordSalt")]
	public string PasswordSalt { get; set; } = default!;

	[Column("Role")]
	public Role Role { get; set; } = default!;
}