using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Web.Controllers.Users
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{

		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[Authorize]
		[HttpGet]
		[Route("all")]
		public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken)
		{
			return await _userService.GetAllUsersAsync(cancellationToken);
		}

		[Authorize]
		[HttpPost]
		public Task CreateUser(CreateUserDto userDto, CancellationToken cancellationToken)
		{
			return _userService.CreateUserAsync(new User
			{
				Login = userDto.Login,
				Email = userDto.Email,
			}, cancellationToken);
		}

		[Authorize]
		[HttpPut]
		public Task UpdateUser(UserDto userDto, CancellationToken cancelationToken)
		{
			return _userService.UpdateUserAsync(new User { Id = userDto.Id, Email = userDto.Email, Login = userDto.Login }, cancelationToken);
		}

		[Authorize]
		[HttpDelete]
		public Task DeleteUser(string id, CancellationToken cancelationToken)
		{
			return _userService.DeleteUserAsync(id, cancelationToken);
		}

	}
}
