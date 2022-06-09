using Minibank.Core.Domains.Accounts;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core
{
	public interface ICurrencyConverter
	{
		Task<double> ConvertCurrencyAsync(double amount, Currencies fromCurrency, Currencies toCurrency, CancellationToken cancellationToken);
	}
}