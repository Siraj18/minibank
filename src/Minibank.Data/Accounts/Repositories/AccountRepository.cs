using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Data.Accounts.Repositories
{
	public class AccountRepository : IAccountRepository
	{
		private readonly MinibankContext _context;
		public AccountRepository(MinibankContext context)
		{
			_context = context;
		}

		public async Task CreateAccountAsync(Account account, CancellationToken cancellationToken)
		{
			var newAccount = new AccountDbModel
			{
				Id = Guid.NewGuid().ToString(),
				UserId = account.UserId,
				Balance = account.Balance,
				Currency = account.Currency,
				IsActive = true,
				CreatedDate = account.CreatedDate
			};

			await _context.Accounts.AddAsync(newAccount, cancellationToken);
		}

		public async Task<List<Account>> GetAllAccountsAsync(CancellationToken cancellationToken)
		{
			var accounts = await _context.Accounts.AsNoTracking().ToListAsync(cancellationToken);

			return accounts.Select(a => new Account
			{
				Id = a.Id,
				UserId = a.UserId,
				Balance = a.Balance,
				Currency = a.Currency,
				CreatedDate = a.CreatedDate,
				ClosedDate = a.ClosedDate,
				IsActive = a.IsActive,
			}).ToList();
		}

		public async Task<Account> GetAccountByIdAsync(string id, CancellationToken cancellationToken)
		{
			var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

			if (account == null)
			{
				return null;
			}

			return new Account
			{
				Id = account.Id,
				UserId = account.UserId,
				Balance = account.Balance,
				Currency = account.Currency,
				IsActive = account.IsActive,
				CreatedDate = account.CreatedDate,
				ClosedDate = account.ClosedDate,

			};
		}

		public async Task<bool> ExistsAccountAsync(string id, CancellationToken cancellationToken)
		{
			var account = await _context.Accounts.AnyAsync(a => a.Id == id, cancellationToken);

			return account;
		}

		public async Task UpdateAccountAsync(Account account, CancellationToken cancellationToken)
		{
			var updateAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id, cancellationToken);

			if (updateAccount == null)
			{
				throw new ObjectNotFoundException();
			}

			updateAccount.Currency = account.Currency;
			updateAccount.Balance = account.Balance;
			updateAccount.IsActive = account.IsActive;
			updateAccount.ClosedDate = account.ClosedDate;
		}
		
		public Task<bool> ExistsAccountsByUserIdAsync(string userId, CancellationToken cancellationToken)
		{
			return _context.Accounts.AnyAsync(a => a.UserId == userId, cancellationToken);
		}
	}
}
