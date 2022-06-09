using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minibank.Web.Controllers.Users.Dto
{
	public class CreateUserDto
	{
		public string Login { get; set; }
		public string Email { get; set; }
	}
}
