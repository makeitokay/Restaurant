using System.Reflection;
using FluentValidation;
using Infrastructure.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

configuration.AddEnvironmentVariables(prefix: "RESTAURANT_");

services.AddControllers();

ValidatorOptions.Global.LanguageManager.Enabled = false;
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

services.AddSingleton<IPasswordManager, PasswordManager>();
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<IUserRepository, UserRepository>();

services.AddJwtAuthentication(configuration.GetSection("Auth"));

services.AddDefaultDatabase(configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();