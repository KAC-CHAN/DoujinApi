namespace DoujinApi.Middlewares;

/**
 * This class represents the API key authentication middleware.
 */
public class ApiKeyAuthMiddleware : IMiddleware
{
	private const string ApiKeyHeaderName = "X-Api-Key";
	private readonly IConfiguration _configuration;

	/**
	 * This constructor initializes the API key authentication middleware.
	 */
	public ApiKeyAuthMiddleware(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	/**
	 * This method invokes the API key authentication middleware.
	 */
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
		{
			context.Response.StatusCode = 401;
			await context.Response.WriteAsync("Missing API Key");
			return;
		}

		string? apiKey = _configuration.GetValue<string>("Authentication:ApiKey");
		if (!apiKey.Equals(potentialApiKey))
		{
			context.Response.StatusCode = 401;
			await context.Response.WriteAsync("Invalid API Key");
			return;
		}

		await next(context);
	}
}