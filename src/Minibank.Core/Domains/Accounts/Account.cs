using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Accounts
{
	public enum Currencies
	{
		RUB,
		USD,
		EUR
	}
	public class Account
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public double Balance { get; set; }
		public Currencies Currency { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ClosedDate { get; set; }
	}
}
