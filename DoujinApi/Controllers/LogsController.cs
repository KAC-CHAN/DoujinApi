using DoujinApi.Models;
using DoujinApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoujinApi.Controllers;

/// <summary>
/// The controller for the logs collection.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class LogsController
{
	private readonly LogService _logService;

	/// <summary>
	/// The constructor for the logs controller.
	/// </summary>
	/// <param name="logService">The log service instance.</param>
	public LogsController(LogService logService)
	{
		_logService = logService;
	}

	/// <summary>
	/// Get all the logs from the database.
	/// </summary>
	/// <returns>An array of all the logs.</returns>
	[HttpGet]
	[Produces("application/json")]
	public async Task<List<Log>> Get() => await _logService.GetAsync();

	/// <summary>
	/// Get the count of all the logs in the database.
	/// </summary>
	/// <returns>The number of logs in the database.</returns>
	[HttpGet("count")]
	public async Task<int> Count() => await _logService.GetCountAsync();

	/// <summary>
	/// Get a log by it's document id.
	/// </summary>
	/// <param name="id">The log's document id.</param>
	/// <returns>A log</returns>
	[HttpGet("{id:length(24)}")]
	public async Task<ActionResult<Log>> Get(string id)
	{
		var log = await _logService.GetAsyncById(id);
		if (log == null)
			return new NotFoundResult();
		return log;
	}

	/// <summary>
	/// Create a new log.
	/// </summary>
	/// <param name="log">The new log to create.</param>
	/// <returns>The log and it's location.</returns>
	[HttpPost]
	public async Task<ActionResult<Log>> Create(Log log)
	{
		await _logService.CreateAsync(log);
		return new CreatedResult($"/api/v1/logs/{log.Id}", log);
	}

	/// <summary>
	/// Delete a log by it's document id.
	/// </summary>
	/// <param name="id">The log's document ID.</param>
	/// <returns>204 on success.</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id)
	{
		var log = await _logService.GetAsyncById(id);
		if (log == null)
			return new NotFoundResult();
		await _logService.DeleteAsync(id);
		return new NoContentResult();
	}

	/// <summary>
	/// Delete all the logs.
	/// </summary>
	/// <returns>204 on success.</returns>
	[HttpDelete]
	public async Task<IActionResult> DeleteAll()
	{
		await _logService.DeleteAllAsync();
		return new NoContentResult();
	}
}