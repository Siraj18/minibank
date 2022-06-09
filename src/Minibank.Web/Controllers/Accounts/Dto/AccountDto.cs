using Minibank.Core.Domains.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minibank.Web.Controllers.Accounts.Dto
{
	public class AccountDto
	{
		public string UserId { get; set; }
		public Currencies Currency { get; set; }
		public double InitialSum { get; set; }
	}

	public class AccountResponseDto
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public double Balance { get; set; }
		public Currencies Currency { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ClosedDate { get; set; }
	}

	public class AccountTransferDto
	{
		public double Amount { get; set; }
		public string FromAccountId { get; set; }
		public string ToAccountId { get; set; }
	}
}
