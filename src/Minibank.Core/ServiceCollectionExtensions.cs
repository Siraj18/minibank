using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Core
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCore(this IServiceCollection services)
		{
			services.AddScoped<ICurrencyConverter, CurrencyConverter>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IDateTimeProvider, DateTimeProvider>();

			services.AddFluentValidation().AddValidatorsFromAssembly(typeof(UserService).Assembly);

			return services;
		}
	}
}
