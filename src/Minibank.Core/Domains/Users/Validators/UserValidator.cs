using FluentValidation;
using Minibank.Core.Domains.Users.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Validators
{
	public class UserValidator : AbstractValidator<User>
	{

		public UserValidator(IUserRepository userRepository)
		{
			RuleFor(u => u.Login).NotEmpty().WithMessage("не должен быть пустым");
			RuleFor(u => u.Login.Length).LessThanOrEqualTo(20).WithMessage("не должен превышать длину в 20 символов");
		}
	}
}
