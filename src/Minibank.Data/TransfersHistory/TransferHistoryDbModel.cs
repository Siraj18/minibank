using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.Accounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Data.TransfersHistory
{
	public class TransferHistoryDbModel
	{
		public string Id { get; set; }
		public double Amount { get; set; }
		public string FromAccountId { get; set; }
		public string ToAccountId { get; set; }
		public virtual AccountDbModel FromAccount { get; set; }
		public virtual AccountDbModel ToAccount { get; set; }

		internal class Map : IEntityTypeConfiguration<TransferHistoryDbModel>
		{
			public void Configure(EntityTypeBuilder<TransferHistoryDbModel> builder)
			{
				builder.ToTable("transfers");

				builder.HasOne(t => t.FromAccount)
					.WithMany(a => a.FromTransfers)
					.HasForeignKey(t => t.FromAccountId);

				builder.HasOne(t => t.ToAccount)
					.WithMany(a => a.ToTransfers)
					.HasForeignKey(t => t.ToAccountId);
			}
		}
	}
}
