using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Core.Domains.Accounts;
using Minibank.Data.TransfersHistory;
using Minibank.Data.Users;
using System;
using System.Collections.Generic;

namespace Minibank.Data.Accounts
{
	public class AccountDbModel
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public double Balance { get; set; }
		public Currencies Currency { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ClosedDate { get; set; }
		public virtual UserDbModel User { get; set; }
		public virtual List<TransferHistoryDbModel> FromTransfers { get; set; }
		public virtual List<TransferHistoryDbModel> ToTransfers { get; set; }
		internal class Map : IEntityTypeConfiguration<AccountDbModel>
		{
			public void Configure(EntityTypeBuilder<AccountDbModel> builder)
			{
				builder.ToTable("accounts");

				builder.HasOne(a => a.User)
					.WithMany(u => u.Accounts)
					.HasForeignKey(a => a.UserId);
			}
		}
	}
}
