using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.TransfersHistory.Repositories
{
	public interface ITransfersHistoriesRepository
	{
		Task CreateTransferHistoryAsync(TransferHistory transferHistory, CancellationToken cancellationToken);
	}
}
