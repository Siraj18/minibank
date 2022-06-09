using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.TransfersHistory;
using Minibank.Core.Domains.TransfersHistory.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Minibank.Core.Domains.Accounts.Services
{
	public class AccountService : IAccountService
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IUserRepository _userRepository;
		private readonly ITransfersHistoriesRepository _transfersHistories;
		private readonly ICurrencyConverter _currencyConverter;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly ILogger<AccountService> _logger;

		public AccountService(IAccountRepository accountRepository, IUserRepository userRepository,
			ICurrencyConverter currencyConverter,
			ITransfersHistoriesRepository transfersHistories,
			IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, ILogger<AccountService> logger)
		{
			_accountRepository = accountRepository;
			_userRepository = userRepository;
			_currencyConverter = currencyConverter;
			_transfersHistories = transfersHistories;
			_unitOfWork = unitOfWork;
			_dateTimeProvider = dateTimeProvider;
			_logger = logger;
		}
		
		public Task<List<Account>> GetAllAccountsAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Return all Accounts");
			
			return _accountRepository.GetAllAccountsAsync(cancellationToken);
		}

		public async Task CloseAccountAsync(string id, CancellationToken cancellationToken)
		{
			var account = await _accountRepository.GetAccountByIdAsync(id, cancellationToken);
			if (account == null)
			{
				throw new ValidationException("Аккаунта с таким id не существует");
			}

			if (account.Balance != 0)
			{
				throw new ValidationException("На аккаунте не нулевой баланс");
			}

			account.IsActive = false;

			var now = _dateTimeProvider.Now;
			account.ClosedDate = now;

			await _accountRepository.UpdateAccountAsync(account, cancellationToken);

			await _unitOfWork.SaveChangesAsync();
			
			_logger.LogInformation("Account closed, Id={id}", id);
		}

		public async Task CreateAccountAsync(Account account, CancellationToken cancellationToken)
		{
			if (!Enum.IsDefined(typeof(Currencies), account.Currency))
			{
				throw new ValidationException("Не поддерживаемая валюта");
			}

			var exist = await _userRepository.ExistsUserAsync(account.UserId, cancellationToken);

			if (!exist)
			{
				throw new ValidationException("Пользователя с таким id не существует");
			}

			var now = _dateTimeProvider.Now;
			account.CreatedDate = now;
			
			await _accountRepository.CreateAccountAsync(account, cancellationToken);

			await _unitOfWork.SaveChangesAsync();
			
			_logger.LogInformation("Account created, UserId={userId}", account.UserId);
		}

		public async Task<double> CalculateCommissionAsync(double amount, string fromAccountId, string toAccountId, CancellationToken cancellationToken)
		{
			var fromAccount = await _accountRepository.GetAccountByIdAsync(fromAccountId, cancellationToken);
			var toAccount = await _accountRepository.GetAccountByIdAsync(toAccountId, cancellationToken);

			if (fromAccount == null || toAccount == null)
			{
				throw new ValidationException("Нет аккаунта с таким id");
			}


			if (fromAccount.UserId == toAccount.UserId)
			{
				return 0;
			}

			var commission = Math.Round(((amount / 100) * 2), 2);
			
			_logger.LogInformation("Calculating commission");
			
			return commission;
		}

		public async Task TransferBalanceAsync(double amount, string fromAccountId, string toAccountId, CancellationToken cancellationToken)
		{
			var exists = await _accountRepository.ExistsAccountAsync(fromAccountId, cancellationToken);
			if (!exists)
			{
				throw new ValidationException("From account с таким id не существует");
			}

			exists = await _accountRepository.ExistsAccountAsync(toAccountId, cancellationToken);
			if (!exists)
			{
				throw new ValidationException("To account с таким id не существует");
			}

			var commission = await CalculateCommissionAsync(amount, fromAccountId, toAccountId, cancellationToken);
			var commissionAmount = amount + commission;

			var fromAccount = await _accountRepository.GetAccountByIdAsync(fromAccountId, cancellationToken);
			var toAccount = await _accountRepository.GetAccountByIdAsync(toAccountId, cancellationToken);


			if (commissionAmount > fromAccount.Balance)
			{
				throw new ValidationException("Недостаточно денег на балансе");
			}

			fromAccount.Balance -= commissionAmount;

			if (fromAccount.Currency != toAccount.Currency)
			{
				amount = await _currencyConverter.ConvertCurrencyAsync(amount, fromAccount.Currency, toAccount.Currency, cancellationToken);
			}

			toAccount.Balance += amount;

			await _accountRepository.UpdateAccountAsync(toAccount, cancellationToken);
			await _accountRepository.UpdateAccountAsync(fromAccount, cancellationToken);

			await _transfersHistories.CreateTransferHistoryAsync(new TransferHistory
			{
				Amount = commissionAmount,
				FromAccountId = fromAccountId,
				ToAccountId = toAccountId,
			}, cancellationToken);

			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Transfer balance, FromAccountId={fromAccountId} ToAccountId={toAccountId}",
				fromAccount, toAccount);
		}
	}
}
