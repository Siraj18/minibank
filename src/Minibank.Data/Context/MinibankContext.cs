using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Minibank.Data.Accounts;
using Minibank.Data.TransfersHistory;
using Minibank.Data.Users;
using System;

namespace Minibank.Data
{
	public class MinibankContext : DbContext
	{
		public DbSet<UserDbModel> Users { get; set; }
		public DbSet<AccountDbModel> Accounts { get; set; }
		public DbSet<TransferHistoryDbModel> Transfers { get; set; }
		public MinibankContext(DbContextOptions options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(MinibankContext).Assembly);
			base.OnModelCreating(modelBuilder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.LogTo(Console.WriteLine);
			base.OnConfiguring(optionsBuilder);
		}

		public class Factory : IDesignTimeDbContextFactory<MinibankContext>
		{
			public MinibankContext CreateDbContext(string[] args)
			{
				var options = new DbContextOptionsBuilder()
					.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=12345")
					.UseSnakeCaseNamingConvention()
					.Options;

				return new MinibankContext(options);
			}
		}
	}

}
