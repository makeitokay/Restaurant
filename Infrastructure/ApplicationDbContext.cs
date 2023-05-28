using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Infrastructure;

public class ApplicationDbContext : DbContext
{
	public DbSet<User> Users => Set<User>();
	public DbSet<Dish> Dishes => Set<Dish>();
	public DbSet<Order> Orders => Set<Order>();
	public DbSet<OrderDish> OrderDishes => Set<OrderDish>();

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder
			.Entity<User>()
			.HasIndex(u => new { u.Email })
			.IsUnique();

		modelBuilder
			.Entity<OrderDish>()
			.HasIndex(od => new { od.OrderId, od.DishId })
			.IsUnique();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseLazyLoadingProxies();
	}
}