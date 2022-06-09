using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Moq;
using Xunit;

namespace Minibank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IAccountRepository> _fakeAccountsRepository;
        private readonly Mock<IUnitOfWork> _fakeIUnitOfWork;
        private readonly Mock<IValidator<User>> _fakeUserValidator;
        private readonly Mock<ILogger<UserService>> _fakeLogger;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeAccountsRepository = new Mock<IAccountRepository>();
            _fakeIUnitOfWork = new Mock<IUnitOfWork>();
            _fakeUserValidator = new Mock<IValidator<User>>();
            _fakeLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_fakeUserRepository.Object, _fakeAccountsRepository.Object,
                _fakeIUnitOfWork.Object, _fakeUserValidator.Object, _fakeLogger.Object);
        }
        
        [Fact]
        public async Task CreateUser_DuplicateLogin_ShouldThrowException()
        {
            //ARRANGE
            _fakeUserRepository
                .Setup(repository => repository.ExistsUserLogin(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            var user = new User { Login = "Login" };
            
            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() => _userService.CreateUserAsync(user, CancellationToken.None));
            
            //ASSERT
            Assert.Equal("Такой логин уже существует", exception.Message);
        }

        [Fact]
        public async Task CreateUser_SuccessPath_ShouldCreateUser()
        {
            //ARRANGE
            var user = new User { Id = "SomeId" };
            
            //ACT
            await _userService.CreateUserAsync(user, CancellationToken.None);
            
            //ASSERT
            _fakeUserRepository.Verify(repository => repository.CreateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task GetById_SuccessPath_ShouldReturnUser()
        {
            //ARRANGE
            var expectedId = "testId";
            var user = new User { Id = expectedId };
            
            _fakeUserRepository
                .Setup(repository => repository.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            //ACT
            var returnUser = await _userService.GetByIdAsync(expectedId, CancellationToken.None);

            //ASSERT
            Assert.Equal(user.Id, returnUser.Id);
        }

        [Fact]
        public async Task DeleteUser_UserWithAccounts_ShouldThrowException()
        {
            //ARRANGE
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountsByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            //ACT
            var exception = await Assert.ThrowsAsync<Exceptions.ValidationException>(() =>
                _userService.DeleteUserAsync("someId", CancellationToken.None));

            //ASSERT
            Assert.Equal("Нельзя удалить пользователя с таким id", exception.Message);
        }

        [Fact]
        public async Task DeleteUser_SuccessPath_ShouldDeleteUser()
        {
            //ARRANGE
            var userId = "someId";
            
            _fakeAccountsRepository
                .Setup(repository => repository.ExistsAccountsByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            
            //ACT
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            //ASSERT
            _fakeUserRepository.Verify(repository => repository.DeleteUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task GetAllUsers_SuccessPath_ShouldReturnUserList()
        {
            //ARRANGE
            _fakeUserRepository
                .Setup(repository => repository.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>()
                {
                    new User { Id = "SomeId" }
                });
            
            //ACT
            var users = await _userService.GetAllUsersAsync(CancellationToken.None);
            
            //ASSERT
            Assert.NotNull(users);
        }

        [Fact]
        public async Task UpdateUser_SuccessPath_ShouldUpdateUser()
        {
            //ARRANGE
            var user = new User { Id = "SomeId" };
            
            //ACT
            await _userService.UpdateUserAsync(user, CancellationToken.None);
            
            //ASSERT
            _fakeUserRepository.Verify(repository => repository.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _fakeIUnitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
        }
        
    }
}