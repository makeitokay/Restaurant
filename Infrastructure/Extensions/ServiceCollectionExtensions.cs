using System.Text;
using Infrastructure.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static TOptions AddOptionsWithValidation<TOptions>(
		this IServiceCollection services,
		IConfigurationSection configurationSection) where TOptions : class, new()
	{
		services
			.AddOptions<TOptions>()
			.Bind(configurationSection)
			.ValidateDataAnnotations()
			.ValidateOnStart();

		var options = new TOptions();
		configurationSection.Bind(options);
		return options;
	}

	public static void AddDefaultDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("Default");
		services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
	}

	public static void AddJwtAuthentication(this IServiceCollection services,
		IConfigurationSection authConfigurationSection)
	{
		var authOptions = services.AddOptionsWithValidation<AuthOptions>(authConfigurationSection);

		services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = authOptions.JwtIssuer,
					ValidAudience = authOptions.JwtAudience,
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.JwtSecretKey)),
					ValidateIssuerSigningKey = true
				};
			});
		services
			.AddAuthorization();
	}
}