using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.TransfersHistory.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Data.Accounts.Repositories;
using Minibank.Data.TransfersHistory.Repositories;
using Minibank.Data.Users.Repositories;
using System;

namespace Minibank.Data
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IAccountRepository, AccountRepository>();
			services.AddScoped<ITransfersHistoriesRepository, TransfersHistoriesRepository>();
			services.AddHttpClient<ICurrencyRateProvider, CurrencyRateProvider>(options =>
			{
				options.BaseAddress = new Uri(configuration["RateUri"]);
			});

			services.AddScoped<IUnitOfWork, EfUnitOfWork>();
			//TODO добавить в конфигурационный файл
			services.AddDbContext<MinibankContext>(options => options
				.UseNpgsql(configuration["ConnectionStrings:MinibankDb"])
				.UseSnakeCaseNamingConvention()
				);

			// docker run -p 5432:5432 --name some-postgresnew -e POSTGRES_PASSWORD=12345 -d postgres:12-alpine

			return services;
		}
	}
}
