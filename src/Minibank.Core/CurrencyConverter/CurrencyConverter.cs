using Minibank.Core.Domains.Accounts;
using Minibank.Core.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core
{
	public class CurrencyConverter : ICurrencyConverter
	{
		private readonly ICurrencyRateProvider _currencyRate;

		public CurrencyConverter(ICurrencyRateProvider currencyRate)
		{
			_currencyRate = currencyRate;
		}

		//TODO подумать как сократить количество обращений к серверу валют в рамках одного запроса
		public async Task<double> ConvertCurrencyAsync(double amount, Currencies fromCurrency, Currencies toCurrency, CancellationToken cancellationToken)
		{
			if (amount < 0)
			{
				throw new ValidationException("Отрицательная сумма");
			}

			double fromRate = 1;
			if (fromCurrency != Currencies.RUB)
			{
				fromRate = await _currencyRate.GetRateAsync(fromCurrency, cancellationToken);
			}

			var fromSum = fromRate * amount;

			if (toCurrency != Currencies.RUB)
			{
				var toRate = await _currencyRate.GetRateAsync(toCurrency, cancellationToken);
				var sum = fromSum / toRate;
				return sum;
			}

			return fromSum;
		}


	}
}
