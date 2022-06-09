using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Accounts.Services
{
	public interface IAccountService
	{
		Task<List<Account>> GetAllAccountsAsync(CancellationToken cancellationToken);
		Task CreateAccountAsync(Account account, CancellationToken cancellationToken);
		Task CloseAccountAsync(string id, CancellationToken cancellationToken);
		Task<double> CalculateCommissionAsync(double amount, string fromAccountId, string toAccountId, CancellationToken cancellationToken);
		Task TransferBalanceAsync(double amount, string fromAccountId, string toAccountId, CancellationToken cancellationToken);
	}
}
