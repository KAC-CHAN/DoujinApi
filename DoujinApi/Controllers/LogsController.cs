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
	/// <param name="ct">Cancellation token</param>
	/// <returns>An array of all the logs.</returns>
	[HttpGet]
	[Produces("application/json")]
	public async Task<List<Log>> Get(CancellationToken ct) => await _logService.GetAsync(ct);

	/// <summary>
	/// Get the count of all the logs in the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The number of logs in the database.</returns>
	[HttpGet("count")]
	public async Task<int> Count(CancellationToken ct) => await _logService.GetCountAsync(ct);

	/// <summary>
	/// Get a log by it's document id.
	/// </summary>
	/// <param name="id">The log's document id.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A log</returns>
	[HttpGet("{id:length(24)}")]
	public async Task<ActionResult<Log>> Get(string id, CancellationToken ct)
	{
		var log = await _logService.GetAsyncById(id,ct);
		if (log == null)
			return new NotFoundResult();
		return log;
	}

	/// <summary>
	/// Create a new log.
	/// </summary>
	/// <param name="log">The new log to create.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The log and it's location.</returns>
	[HttpPost]
	public async Task<ActionResult<Log>> Create(Log log, CancellationToken ct)
	{
		await _logService.CreateAsync(log, ct);
		return new CreatedResult($"/api/v1/logs/{log.Id}", log);
	}

	/// <summary>
	/// Delete a log by it's document id.
	/// </summary>
	/// <param name="id">The log's document ID.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>204 on success.</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id, CancellationToken ct)
	{
		var log = await _logService.GetAsyncById(id, ct);
		if (log == null)
			return new NotFoundResult();
		await _logService.DeleteAsync(id, ct);
		return new NoContentResult();
	}

	/// <summary>
	/// Delete all the logs.
	/// </summary>
	/// <returns>204 on success.</returns>
	/// <param name="ct">Cancellation token</param>
	[HttpDelete]
	public async Task<IActionResult> DeleteAll(CancellationToken ct)
	{
		await _logService.DeleteAllAsync(ct);
		return new NoContentResult();
	}
}