using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.TransfersHistory
{
	public class TransferHistory
	{
		public string Id { get; set; }
		public double Amount { get; set; }
		public string FromAccountId { get; set; }
		public string ToAccountId { get; set; }

	}
}
