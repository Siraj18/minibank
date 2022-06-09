using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Data.Users.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly MinibankContext _context;
		public UserRepository(MinibankContext context)
		{
			_context = context;
		}

		public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
		{
			var newUser = new UserDbModel
			{
				Id = Guid.NewGuid().ToString(),
				Login = user.Login,
				Email = user.Email,
			};

			await _context.Users.AddAsync(newUser, cancellationToken);
		}

		public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
		{
			var updateUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

			if (updateUser == null)
			{
				throw new ObjectNotFoundException();
			}

			updateUser.Login = user.Login;
			updateUser.Email = user.Email;
		}

		public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
		{
			var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

			if (getUser == null)
			{
				throw new ObjectNotFoundException();
			}

			return new User
			{
				Id = getUser.Id,
				Login = getUser.Login,
				Email = getUser.Email,
			};
		}

		public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
		{
			var users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);

			return users.Select(u => new User { Id = u.Id, Login = u.Login, Email = u.Email }).ToList();
		}

		public async Task DeleteUserAsync(string id, CancellationToken cancellationToken)
		{
			var deleteUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

			if (deleteUser == null)
			{
				throw new ObjectNotFoundException();
			}

			_context.Users.Remove(deleteUser);
		}

		public async Task<bool> ExistsUserAsync(string id, CancellationToken cancellationToken)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
			if (user == null)
			{
				return false;
			}

			return true;
		}

		public Task<bool> ExistsUserLogin(string login, CancellationToken cancellationToken)
		{
			return _context.Users.AnyAsync(u => u.Login == login, cancellationToken);
		}

	}
}
