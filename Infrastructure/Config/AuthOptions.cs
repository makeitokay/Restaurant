using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Config;

public class AuthOptions
{
	[Required]
	public string JwtIssuer { get; set; }
	
	[Required]
	public string JwtAudience { get; set; }
	
	[Required]
	public string JwtSecretKey { get; set; }
}