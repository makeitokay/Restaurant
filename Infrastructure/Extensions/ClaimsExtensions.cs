using System.Security.Claims;

namespace Infrastructure.Extensions;

public static class ClaimsExtensions
{
	public static string GetEmail(this IEnumerable<Claim> claims)
	{
		return claims.GetClaimValue(ClaimTypes.Email);
	}

	private static string GetClaimValue(this IEnumerable<Claim> claims, string claimType)
	{
		return claims.FirstOrDefault(claim => claim.Type == claimType)?.Value
		       ?? throw new ArgumentException($"No value for claim type = `{claimType}`", nameof(claimType));
	}
}