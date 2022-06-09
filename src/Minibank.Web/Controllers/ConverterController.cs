using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core;
using Minibank.Core.Domains.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConverterController : ControllerBase
	{
		private readonly ICurrencyConverter _converter;

		public ConverterController(ICurrencyConverter converter)
		{
			_converter = converter;
		}

		[Authorize]
		[HttpGet]
		public Task<double> ConvertCurrency(double amount, Currencies fromCurrency, Currencies toCurrency, CancellationToken cancellationToken)
		{
			return _converter.ConvertCurrencyAsync(amount, fromCurrency, toCurrency, cancellationToken);
		}
	}
}
