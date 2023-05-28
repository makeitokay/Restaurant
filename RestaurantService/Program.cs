using System.Reflection;
using FluentValidation;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using RestaurantService.Tasks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

configuration.AddEnvironmentVariables(prefix: "RESTAURANT_");

services.AddControllers();

ValidatorOptions.Global.LanguageManager.Enabled = false;
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<IUserRepository, UserRepository>();

services.AddJwtAuthentication(configuration.GetSection("Auth"));

services.AddDefaultDatabase(configuration);

services.AddHostedService<OrdersProcessingBackgroundTask>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var dbContext = app.Services.GetRequiredService<ApplicationDbContext>();
if (dbContext.Database.GetPendingMigrations().Any())
{
	await dbContext.Database.MigrateAsync();
}

app.Run();