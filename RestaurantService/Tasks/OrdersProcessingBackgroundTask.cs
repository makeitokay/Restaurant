using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace RestaurantService.Tasks;

public class OrdersProcessingBackgroundTask : BackgroundService
{
	private IServiceProvider _serviceProvider;
	private readonly ILogger<OrdersProcessingBackgroundTask> _logger;

	private static readonly TimeSpan _runInterval = TimeSpan.FromSeconds(15);

	public OrdersProcessingBackgroundTask(
		IServiceProvider serviceProvider,
		ILogger<OrdersProcessingBackgroundTask> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await ExecuteIterationAsync();
			}
			catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested) { }
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
			catch (Exception exception)
			{
				_logger.LogError(exception, "Failed to execute iteration");
			}

			await Task.Delay(_runInterval, stoppingToken);
		}
	}

	private async Task ExecuteIterationAsync()
	{
		_logger.LogInformation("Start executing order processing iteration");

		using var scope = _serviceProvider.CreateScope();

		var orderRepository = scope.ServiceProvider.GetRequiredService<IRepository<Order>>();
		
		var ordersToProcess = await orderRepository
			.Items
			.Where(o => o.Status == OrderStatus.Queued || o.Status == OrderStatus.InProgress)
			.ToListAsync();

		foreach (var order in ordersToProcess)
		{
			order.Status = order.Status switch
			{
				OrderStatus.Queued => OrderStatus.InProgress,
				OrderStatus.InProgress => OrderStatus.Finished,
				_ => order.Status
			};
		}

		_logger.LogInformation("Executed order processing iteration");
		await orderRepository.UpdateRangeAsync(ordersToProcess);
	}
}