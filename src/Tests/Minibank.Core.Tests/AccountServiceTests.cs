using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Minibank.Core;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.TransfersHistory;
using Minibank.Core.Domains.TransfersHistory.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Moq;
using Xunit;

namespace Minibank.Core.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _fakeAccountsRepository;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<ITransfersHistoriesRepository> _fakeTransfersRepository;
        private readonly Mock<IUnitOfWork> _fakeIUnitOfWork;
        private readonly Mock<ICurrencyConverter> _fakeCurrencyConverter;
        private readonly Mock<IDateTimeProvider> _fakeDateTimeProvider;
        private readonly Mock<ILogger<AccountService>> _fakeLogger;
        private readonly IAccountService _accountService;
        
        public AccountServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeAccountsRepository = new Mock<IAccountRepository>();
            _fakeIUnitOfWork = new Mock<IUnitOfWork>();
            _fakeTransfersRepository = new Mock<ITransfersHistoriesRepository>();
            _fakeCurrencyConverter = new Mock<ICurrencyConverter>();
            _fakeDateTimeProvider = new Mock<IDateTimeProvider>();
            _fakeLogger = new Mock<ILogger<AccountService>>();
            _accountService = new AccountService(_fakeAccountsRepository.Object, _fakeUserRepository.Object,
                _fakeCurrencyConverter.Object,
                _fakeTransfersRepository.Object, _fakeIUnitOfWork.Object, _fakeDateTimeProvider.Object,
                _fakeLogger.Object);
        }

        [Fact]
        public async Task GetAllAccounts_SuccessPath_ShouldReturnAccounts()
        {
            //ARRANGE
            _fakeAccountsRepository
                .Setup(repository => repository.GetAllAccountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Account>()
                {
                    new Account { Id = "SomeId" }
                });
            
            //ACT
            var accounts = await _accountService.GetAllAccountsAsync(CancellationToken.None);
            
            //ASSERT
            Assert.NotNull(accounts);
        }

        [Fact]
        public async Task CloseAccount_WrongId_ShouldThrowException()
        {
            //ARRANGE
            var wrongId = "wrongId";
            var exceptionMessage = "Аккаунта с таким id не существует";
            Account nullAccount = null;

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nullAccount);
            
            //ACT
            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() => _accountService.CloseAccountAsync(wrongId, CancellationToken.None));

            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task CloseAccount_NotNullBalance_ShouldThrowException()
        {
            //ARRANGE
            var someId = "someId";
            var exceptionMessage = "На аккаунте не нулевой баланс";
            
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account { Balance = 100 });
            
            //ACT
            var exception =
                await Assert.ThrowsAsync<Exceptions.ValidationException>(() => _accountService.CloseAccountAsync(someId, CancellationToken.None));

            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task CloseAccount_SuccessPath_ShouldCloseAccount()
        {
            //ARRANGE
            var someId = "someId";
          
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account { Balance = 0 });
            
            //ACT
            await _accountService.CloseAccountAsync(someId, CancellationToken.None);
            
            //ASSERT
            _fakeAccountsRepository.Verify(
                repository => repository.UpdateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task CloseAccount_SuccessPath_ShouldSetClosedTime()
        {
            //ARRANGE
            var someId = "someId";
            var someAccount = new Account { Id = someId };
            
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account { Balance = 0 });
            
            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => someAccount = ac);
            
            var expectedDate = new DateTime(2000, 1, 1);
            _fakeDateTimeProvider.Setup(provider => provider.Now).Returns(expectedDate);
            
            //ACT
            await _accountService.CloseAccountAsync(someId, CancellationToken.None);
            
            //ASSERT
            Assert.Equal(expectedDate, someAccount.ClosedDate);
        }

        [Fact]
        public async Task CreateAccount_WrongUserId_ShouldThrowException()
        {
            //ARRANGE
            var account = new Account { UserId = "wrongId" };
            var exceptionMessage = "Пользователя с таким id не существует";
            
            _fakeUserRepository
                .Setup(repository => repository.ExistsUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            
            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _accountService.CreateAccountAsync(account, CancellationToken.None));
            
            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task CreateAccount_SuccessPath_ShouldCreateAccount()
        {
            //ARRANGE
            var account = new Account { UserId = "SomeId" };
            
            _fakeUserRepository
                .Setup(repository => repository.ExistsUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            //ACT
            await _accountService.CreateAccountAsync(account, CancellationToken.None);
            
            //ASSERT
            _fakeAccountsRepository.Verify(
                repository => repository.CreateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task CreateAccount_SuccessPath_ShouldSetCreatedDate()
        {
            //ARRANGE
            var account = new Account { UserId = "SomeId" };
            
            _fakeUserRepository
                .Setup(repository => repository.ExistsUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => account = ac);
            
            var expectedDate = new DateTime(2000, 1, 1);
            _fakeDateTimeProvider.Setup(provider => provider.Now).Returns(expectedDate);
            
            //ACT
            await _accountService.CreateAccountAsync(account, CancellationToken.None);
            
            //ASSERT
            Assert.Equal(expectedDate, account.CreatedDate);
        }

        [Fact]
        public async Task CalculateCommission_WrongFromId_ShouldThrowException()
        {
            //ARRANGE
            var fromAccountId = "wrongId";
            var toAccountId = "someId";
            var exceptionMessage = "Нет аккаунта с таким id";
            Account nullAccount = null;

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nullAccount);

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account
                {
                    Id = toAccountId
                });

            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _accountService.CalculateCommissionAsync(1, fromAccountId, toAccountId, CancellationToken.None));
            
            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task CalculateCommission_WrongToId_ShouldThrowException()
        {
            //ARRANGE
            var fromAccountId = "some Id";
            var toAccountId = "wrongId";
            var exceptionMessage = "Нет аккаунта с таким id";
            Account nullAccount = null;

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account
                {
                    Id = fromAccountId
                });

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nullAccount);

            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _accountService.CalculateCommissionAsync(1, fromAccountId, toAccountId, CancellationToken.None));
            
            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task CalculateCommission_WithSameUserId_ShouldCalculateComission()
        {
            //ARRANGE
            var fromAccountId = "fromId";
            var toAccountId = "toId";
            var userId = "userId";
            var amount = 100;
            var correctCommission = 0;

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account
                {
                    Id = fromAccountId,
                    UserId = userId
                });

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new Account
                    {
                        Id = toAccountId,
                        UserId = userId
                    });

            //ACT
            var commission = await _accountService.CalculateCommissionAsync(amount, fromAccountId, toAccountId,
                CancellationToken.None);
            
            //ASSERT
            Assert.Equal(correctCommission, commission);
        }
        
        [Fact]
        public async Task CalculateCommission_WithDifferentUserId_ShouldCalculateComission()
        {
            //ARRANGE
            var fromAccountId = "fromId";
            var toAccountId = "toId";
            double amount = 100;
            var correctCommission = Math.Round(((amount / 100) * 2), 2);

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account
                {
                    Id = fromAccountId,
                    UserId = "userId1"
                });

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new Account
                    {
                        Id = toAccountId,
                        UserId = "userId2"
                    });

            //ACT
            var commission = await _accountService.CalculateCommissionAsync(amount, fromAccountId, toAccountId,
                CancellationToken.None);
            
            //ASSERT
            Assert.Equal(correctCommission, commission);
        }

        [Fact]
        public async Task TransferBalance_WrongFromId_ShouldThrowException()
        {
            //ARRANGE
            var fromAccountId = "wrongFromId";
            var toAccountId = "toId";
            var exceptionMessage = "From account с таким id не существует";
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _accountService.TransferBalanceAsync(1, fromAccountId, toAccountId, CancellationToken.None));

            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task TransferBalance_WrongToId_ShouldThrowException()
        {
            //ARRANGE
            var fromAccountId = "fromId";
            var toAccountId = "wrongToId";
            var exceptionMessage = "To account с таким id не существует";
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _accountService.TransferBalanceAsync(1, fromAccountId, toAccountId, CancellationToken.None));

            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
        [Fact]
        public async Task TransferBalance_NotEnoughMoney_ShouldThrowException()
        {
            //ARRANGE
            var fromAccountId = "fromId";
            var toAccountId = "wrongToId";
            var exceptionMessage = "Недостаточно денег на балансе";
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(fromAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(toAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
             
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account
                {
                    Id = fromAccountId,
                    UserId = "userId1",
                    Balance = 0,
                });

            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _accountService.TransferBalanceAsync(1, fromAccountId, toAccountId, CancellationToken.None));

            //ASSERT
            Assert.Equal(exceptionMessage, exception.Message);
        }
        
         [Fact]
        public async Task TransferBalance_DifferentCurrency_ShouldTransferBalance()
        {
            //ARRANGE
            double amount = 100;
            double amountInOtherCurrency = 2;

            var fromAccount = new Account
            {
                Id = "fromId",
                UserId = "userId1",
                Currency = Currencies.RUB,
                Balance = 100,
            };
            
            var toAccount = new Account
            {
                Id = "toId",
                UserId = "userId1",
                Currency = Currencies.USD,
                Balance = 100,
            };

            var correctToAccountBalance = toAccount.Balance + amountInOtherCurrency;
            var correctFromAccountBalance = fromAccount.Balance - amount;

            _fakeCurrencyConverter
                .Setup(converter => converter.ConvertCurrencyAsync(amount, fromAccount.Currency, toAccount.Currency,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(amountInOtherCurrency);
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(fromAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(toAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
             
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);
            
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);

            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(toAccount, It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => toAccount = ac);
            
            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(fromAccount, It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => fromAccount = ac);

            //ACT
            await _accountService.TransferBalanceAsync(amount, fromAccount.Id, toAccount.Id, CancellationToken.None);

            //ASSERT
            Assert.Equal(correctToAccountBalance, toAccount.Balance);
            Assert.Equal(correctFromAccountBalance, fromAccount.Balance);
            
            _fakeAccountsRepository.Verify(
                repository =>
                    repository.UpdateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            
            _fakeTransfersRepository.Verify(
                repository =>
                    repository.CreateTransferHistoryAsync(It.IsAny<TransferHistory>(), CancellationToken.None),
                Times.Once);
            
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }

        
        // Разобраться с commision
        [Fact]
        public async Task TransferBalance_WithSameCurrencyAndSameUserId_ShouldTransferBalance()
        {
            //ARRANGE
            double amount = 100;

            var fromAccount = new Account
            {
                Id = "fromId",
                UserId = "userId1",
                Balance = 100,
            };
            
            var toAccount = new Account
            {
                Id = "toId",
                UserId = "userId1",
                Balance = 100,
            };

            var correctToAccountBalance = toAccount.Balance + amount;
            var correctFromAccountBalance = fromAccount.Balance - amount;
            
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(fromAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(toAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
             
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);
            
            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);

            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(toAccount, It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => toAccount = ac);
            
            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(fromAccount, It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => fromAccount = ac);

            //ACT
            await _accountService.TransferBalanceAsync(amount, fromAccount.Id, toAccount.Id, CancellationToken.None);

            //ASSERT
            Assert.Equal(correctToAccountBalance, toAccount.Balance);
            Assert.Equal(correctFromAccountBalance, fromAccount.Balance);
            
            _fakeAccountsRepository.Verify(
                repository =>
                    repository.UpdateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            
            _fakeTransfersRepository.Verify(
                repository =>
                    repository.CreateTransferHistoryAsync(It.IsAny<TransferHistory>(), CancellationToken.None),
                Times.Once);
            
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task TransferBalance_WithSameCurrencyAndDifferentUserId_ShouldTransferBalance()
        {
            //ARRANGE
            double amount = 100;


            var fromAccount = new Account
            {
                Id = "fromId",
                UserId = "userId1",
                Balance = 200,
            };

            var toAccount = new Account
            {
                Id = "toId",
                UserId = "userId2",
                Balance = 100,
            };

            var correctCommission = Math.Round(((amount / 100) * 2), 2);

            var correctToAccountBalance = toAccount.Balance + amount;
            var correctFromAccountBalance = fromAccount.Balance - amount - correctCommission;


            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(fromAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountAsync(toAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(toAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(toAccount);

            _fakeAccountsRepository
                .Setup(repository => repository.GetAccountByIdAsync(fromAccount.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fromAccount);

            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(toAccount, It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => toAccount = ac);

            _fakeAccountsRepository
                .Setup(repository => repository.UpdateAccountAsync(fromAccount, It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((ac, t) => fromAccount = ac);

            //ACT
            await _accountService.TransferBalanceAsync(amount, fromAccount.Id, toAccount.Id, CancellationToken.None);

            //ASSERT
            Assert.Equal(correctToAccountBalance, toAccount.Balance);
            Assert.Equal(correctFromAccountBalance, fromAccount.Balance);

            _fakeAccountsRepository.Verify(
                repository =>
                    repository.UpdateAccountAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            _fakeTransfersRepository.Verify(
                repository =>
                    repository.CreateTransferHistoryAsync(It.IsAny<TransferHistory>(), CancellationToken.None),
                Times.Once);

            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }
    }
}