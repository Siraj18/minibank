using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Accounts.Repositories
{
	public interface IAccountRepository
	{
		Task<List<Account>> GetAllAccountsAsync(CancellationToken cancellationToken);
		Task CreateAccountAsync(Account account, CancellationToken cancellationToken);
		Task<Account> GetAccountByIdAsync(string id, CancellationToken cancellationToken);
		Task UpdateAccountAsync(Account account, CancellationToken cancellationToken);
		Task<bool> ExistsAccountAsync(string id, CancellationToken cancellationToken);
		Task<bool> ExistsAccountsByUserIdAsync(string userId, CancellationToken cancellationToken);
	}
}
