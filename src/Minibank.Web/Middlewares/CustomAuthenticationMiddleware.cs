using Microsoft.AspNetCore.Http;
using Minibank.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Minibank.Web.Middlewares
{
	public class CustomAuthenticationMiddleware
	{
		private readonly RequestDelegate _next;
		public CustomAuthenticationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var authHeader = httpContext.Request.Headers["Authorization"];
			if (authHeader.Count != 0 )
			{
				var jwtData = authHeader[0].Split(" ");

				var handler = new JwtSecurityTokenHandler();
				var jwtSecurityToken = handler.ReadJwtToken(jwtData[1]);

				var expClaim = jwtSecurityToken.Claims.First(c => c.Type == "exp").Value;

				var dateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim));

				if (dateTime <= DateTimeOffset.Now)
				{
					throw new TokenExpiredException();
				}
			}
			
			await _next(httpContext);
		}
	}
}
