using Minibank.Core.Domains.TransfersHistory;
using Minibank.Core.Domains.TransfersHistory.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Data.TransfersHistory.Repositories
{
	public class TransfersHistoriesRepository : ITransfersHistoriesRepository
	{
		private readonly MinibankContext _context;
		public TransfersHistoriesRepository(MinibankContext context)
		{
			_context = context;
		}

		public async Task CreateTransferHistoryAsync(TransferHistory transferHistory, CancellationToken cancellationToken)
		{
			await _context.Transfers.AddAsync(new TransferHistoryDbModel
			{
				Id = Guid.NewGuid().ToString(),
				Amount = transferHistory.Amount,
				FromAccountId = transferHistory.FromAccountId,
				ToAccountId = transferHistory.ToAccountId,
			}, cancellationToken);
		}
	}
}
