using System.Net;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TelegramBotApi.Exceptions;
using TelegramBotApi.Services;
using LogLevel = TelegramBotApi.Models.LogLevel;

namespace TelegramBotApi.Middlewares;

/// <summary>
/// Global error handling middleware.
/// </summary>
public class GlobalExeptionHandlerMiddleware : IMiddleware
{
	private readonly LoggerService _loggerService;
	
	/// <summary>
	/// Middleware constructor.
	/// </summary>
	/// <param name="loggerService">The logger service</param>
	public GlobalExeptionHandlerMiddleware(LoggerService loggerService)
	{
		_loggerService = loggerService;
	}
	
	/// <summary>
	/// Error handling middleware.
	/// </summary>
	/// <param name="context">Http context.</param>
	/// <param name="next">Request delegate</param>
	/// <returns>An appropriate error message to the client</returns>
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (Exception e)
		{
			await _loggerService.Log(LogLevel.Error, e.Message);
			context.Response.ContentType = "application/json";
			if (e is ExhentaiException exhentaiException)
			{
				context.Response.StatusCode = (int) exhentaiException.HttpCode;

				var problem = new ProblemDetails
				{
					Title = exhentaiException.Name.ToString(),
					Detail = exhentaiException.Message,
					Status = (int) exhentaiException.HttpCode,
					Instance = HttpUtility.UrlDecode(context.Request.Path)
				};

				await context.Response.WriteAsync(JsonSerializer.Serialize(problem));

			}
			else
			{
				context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
				
				var problem = new ProblemDetails
				{
					Title = "Internal Server Error",
					Detail = e.Message,
					Status = (int) HttpStatusCode.InternalServerError,
					Instance = HttpUtility.UrlDecode(context.Request.Path)
				};
				
				await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
			}
		}
		
	}
}