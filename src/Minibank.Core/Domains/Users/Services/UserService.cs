using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Minibank.Core.Domains.Users.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<User> _userValidator;
		private readonly ILogger<UserService> _logger;

		public UserService(IUserRepository userRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork,
			IValidator<User> userValidator, ILogger<UserService> logger)
		{
			_userRepository = userRepository;
			_accountRepository = accountRepository;
			_unitOfWork = unitOfWork;
			_userValidator = userValidator;
			_logger = logger;
		}

		public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
		{
			_userValidator.ValidateAndThrow(user);

			var loginExist = await _userRepository.ExistsUserLogin(user.Login, cancellationToken);

			if (loginExist)
			{
				throw new Exceptions.ValidationException("Такой логин уже существует");
			}

			await _userRepository.CreateUserAsync(user, cancellationToken);

			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("User created, Login={Login}", user.Login);
		}

		public async Task DeleteUserAsync(string id, CancellationToken cancellationToken)
		{
			var exist = await _accountRepository.ExistsAccountsByUserIdAsync(id, cancellationToken);
			
			if (exist)
			{
				throw new Exceptions.ValidationException("Нельзя удалить пользователя с таким id");
			}
			await _userRepository.DeleteUserAsync(id, cancellationToken);

			await _unitOfWork.SaveChangesAsync();
			
			_logger.LogInformation("User deleted, Id={Id}", id);
		}

		public Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Return all users");
			
			return _userRepository.GetAllUsersAsync(cancellationToken);
		}

		public Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Return User, Id={id}", id);
			
			return _userRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
		{
			await _userRepository.UpdateUserAsync(user, cancellationToken);

			await _unitOfWork.SaveChangesAsync();
			
			_logger.LogInformation("User updated, Id={Id}", user.Id);
		}
	}
}
