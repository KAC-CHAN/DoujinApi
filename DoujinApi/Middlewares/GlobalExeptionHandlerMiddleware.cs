using System.Net;
using System.Text.Json;
using System.Web;
using DoujinApi.Exceptions;
using DoujinApi.Services;
using Kvyk.Telegraph.Exceptions;
using Microsoft.AspNetCore.Mvc;
using LogLevel = DoujinApi.Models.LogLevel;

namespace DoujinApi.Middlewares;

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
			var problem = new ProblemDetails();
			
			switch (e)
			{
				case ExhentaiException exhentaiException:
					context.Response.StatusCode = (int) exhentaiException.HttpCode;
					problem.Title = exhentaiException.Name.ToString();
					problem.Detail = exhentaiException.Message;
					problem.Status = (int) exhentaiException.HttpCode;
					problem.Instance = HttpUtility.UrlDecode(context.Request.Path);
					break;
				case TelegraphException telegraphException:
					context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
					problem.Title = "TelegraphExceception";
					problem.Detail = telegraphException.Message;
					problem.Status = (int) HttpStatusCode.InternalServerError;
					problem.Instance = HttpUtility.UrlDecode(context.Request.Path);
					break;
				default:
					context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
					problem.Title = "Internal Server Error";
					problem.Detail = e.Message;
					problem.Status = (int) HttpStatusCode.InternalServerError;
					problem.Instance = HttpUtility.UrlDecode(context.Request.Path);
					break;
			}
			await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
		}
		
	}
}