using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Data.Users
{
	public class UserDbModel
	{
		public string Id { get; set; }
		public string Login { get; set; }
		public string Email { get; set; }
		public virtual List<AccountDbModel> Accounts { get; set; }

		internal class Map : IEntityTypeConfiguration<UserDbModel>
		{
			public void Configure(EntityTypeBuilder<UserDbModel> builder)
			{
				builder.ToTable("users");
			}
		}
	}
}
