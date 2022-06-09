using Minibank.Core.Domains.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core
{
	public interface ICurrencyRateProvider
	{
		Task<double> GetRateAsync(Currencies currency, CancellationToken cancellationToken);
	}
}
