using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Web.Controllers.Accounts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Web.Controllers.Accounts
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly IAccountService _accountService;
		public AccountsController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		// Данный эндпоинт только для проверки
		[Authorize]
		[HttpGet]
		[Route("all")]
		public async Task<List<Account>> GetAllAcounts(CancellationToken cancellationToken)
		{
			return await _accountService.GetAllAccountsAsync(cancellationToken);
		}

		[Authorize]
		[HttpPost]
		public Task CreateAccount(AccountDto accountDto, CancellationToken cancellationToken)
		{
			return _accountService.CreateAccountAsync(
					new Account { UserId = accountDto.UserId, Currency = accountDto.Currency, Balance = accountDto.InitialSum },
					cancellationToken
				);
		}

		[Authorize]
		[HttpGet]
		[Route("commission")]
		public Task<double> CalculateCommission(double amount, string fromAccountId, string toAccountId, CancellationToken cancellationToken)
		{
			return _accountService.CalculateCommissionAsync(amount, fromAccountId, toAccountId, cancellationToken);
		}

		[Authorize]
		[HttpPut]
		[Route("close")]
		public Task CloseAccount(string id, CancellationToken cancellationToken)
		{
			return _accountService.CloseAccountAsync(id, cancellationToken);
		}

		[Authorize]
		[HttpPost]
		[Route("transfer")]
		public Task TransferBalance(AccountTransferDto transferDto, CancellationToken cancellationToken)
		{
			return _accountService.TransferBalanceAsync(transferDto.Amount, transferDto.FromAccountId, transferDto.ToAccountId, cancellationToken);
		}
	}
}
