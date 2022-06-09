using Minibank.Core;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Exceptions;
using Minibank.Data.HttpClients.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Data
{
	public class CurrencyRateProvider : ICurrencyRateProvider
	{
		private readonly HttpClient _httpClient;
		public CurrencyRateProvider(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<double> GetRateAsync(Currencies currency, CancellationToken cancellationToken)
		{
			var response = await _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js", cancellationToken);

			if (!response.Valute.ContainsKey(currency.ToString()))
			{
				throw new ValidationException("Неверная валюта");
			}

			return response.Valute[currency.ToString()].Value;
		}

	}
}
