using Microsoft.AspNetCore.Http;
using Minibank.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Minibank.Web.Middlewares
{
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<LoggingMiddleware> _logger;
		public LoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger<LoggingMiddleware>();
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var loggerParams = new Dictionary<string, object>()
			{
				{"User-Agent", httpContext.Request.Headers["User-Agent"]}
			};
			
			using (_logger.BeginScope(loggerParams))
			{
				await _next(httpContext);
			}
			
		}
	}
}
