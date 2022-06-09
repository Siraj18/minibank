using Microsoft.AspNetCore.Http;
using Minibank.Core.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Minibank.Web.Middlewares
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
		{
			try
			{
				await _next(httpContext);
			}
			catch (TokenExpiredException exception)
			{
				httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
				await httpContext.Response.WriteAsJsonAsync(new { } );
			}
			catch (ObjectNotFoundException exception)
			{
				httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
				await httpContext.Response.WriteAsJsonAsync(new { Error = "Object Not Found" });
			}
			catch (ValidationException exception)
			{
				httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
				await httpContext.Response.WriteAsJsonAsync(new { Message = exception.Message });
			}
			catch (FluentValidation.ValidationException exception)
			{
				var errors = exception.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");

				var errorMessage = string.Join(Environment.NewLine, errors);

				httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
				await httpContext.Response.WriteAsJsonAsync(new { Error = errorMessage });
			}
			catch (Exception exception)
			{
				httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
				logger.LogError("Unhandled exception, Message={message}", exception.Message);
				await httpContext.Response.WriteAsJsonAsync(new { Error = "Внутренняя ошибка сервера" });
			}
		}
	}
}
