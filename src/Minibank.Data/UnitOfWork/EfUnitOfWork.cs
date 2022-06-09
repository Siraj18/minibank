using Minibank.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minibank.Data
{
	public class EfUnitOfWork : IUnitOfWork
	{
		private readonly MinibankContext _context;

		public EfUnitOfWork(MinibankContext context)
		{
			_context = context;
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
