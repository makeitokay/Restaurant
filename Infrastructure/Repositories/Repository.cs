using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Infrastructure.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
	Task<TEntity> CreateAsync(TEntity entity);
	Task CreateRangeAsync(IEnumerable<TEntity> entities);
	Task<TEntity> UpdateAsync(TEntity entity);
	Task UpdateRangeAsync(IEnumerable<TEntity> entities);
	Task DeleteAsync(TEntity entity);
	Task<TEntity> GetAsync(int id);
	Task<TEntity?> FindAsync(int id);
	IQueryable<TEntity> Items { get; }
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
	private readonly ApplicationDbContext _dbContext;

	private readonly DbSet<TEntity> _set;

	public IQueryable<TEntity> Items => _set.AsQueryable();

	public Repository(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
		_set = dbContext.Set<TEntity>();
	}

	public async Task<TEntity> CreateAsync(TEntity entity)
	{
		AddEntityToSet(entity);
		await _dbContext.SaveChangesAsync();
		return entity;
	}

	public async Task CreateRangeAsync(IEnumerable<TEntity> entities)
	{
		foreach (var entity in entities)
		{
			AddEntityToSet(entity);
		}

		await _dbContext.SaveChangesAsync();
	}

	public async Task<TEntity> UpdateAsync(TEntity entity)
	{
		UpdateEntityInSet(entity);
		await _dbContext.SaveChangesAsync();
		return entity;
	}

	public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
	{
		foreach (var entity in entities)
		{
			UpdateEntityInSet(entity);
		}

		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(TEntity entity)
	{
		_set.Remove(entity);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<TEntity> GetAsync(int id)
	{
		return await FindAsync(id) ?? throw new ArgumentException($"Entity with id = {id} not found");
	}
	
	public async Task<TEntity?> FindAsync(int id)
	{
		return await _set.FindAsync(id);
	}

	private void AddEntityToSet(TEntity entity)
	{
		if (entity is BaseAuditableEntity auditableEntity)
		{
			auditableEntity.CreatedAtUtc = DateTimeOffset.UtcNow;
			auditableEntity.UpdatedAtUtc = auditableEntity.CreatedAtUtc;
		}
		_set.Add(entity);
	}

	private void UpdateEntityInSet(TEntity entity)
	{
		if (entity is BaseAuditableEntity auditableEntity)
		{
			auditableEntity.UpdatedAtUtc = DateTimeOffset.UtcNow;
		}
		_set.Update(entity);
	}
}