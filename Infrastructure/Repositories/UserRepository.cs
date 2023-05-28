using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Infrastructure.Repositories;

public interface IUserRepository : IRepository<User>
{
	Task<User?> FindUserByEmailAsync(string email);
}


public class UserRepository : Repository<User>, IUserRepository
{
	public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
	{
	}

	public async Task<User?> FindUserByEmailAsync(string email)
	{
		return await Items
			.SingleOrDefaultAsync(u => u.Email == email);
	}
}