using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TelegramBotApi.Models;
using TelegramBotApi.Services;
using TelegramBotApi.Sources;

namespace TelegramBotApi.Controllers;

/// <summary>
/// Controller for the doujins collection.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class DoujinsController
{
	
	private readonly DoujinService _doujinService;
	private readonly LoggerService _loggerService;
	private readonly SettingService _settingService;
	private readonly StatsService _statsService;

	/// <summary>
	///  Constructor for the doujins controller.
	/// </summary>
	/// <param name="doujinService">The DoujinService Object.</param>
	public DoujinsController(DoujinService doujinService, LoggerService loggerService, SettingService settingService, StatsService statsService)
	{
		_doujinService = doujinService;
		_loggerService = loggerService;
		_settingService = settingService;
		_statsService = statsService;
	}

	/// <summary>
	/// Get all the doujins inside the database.
	/// </summary>
	/// <returns>All the doujins.</returns>
	[HttpGet]
	[Produces("application/json")]
	public async Task<List<Doujin>> Get() => await _doujinService.GetAsync();

	/// <summary>
	/// Get a doujin by its document id.
	/// </summary>
	/// <param name="id">The document id.</param>
	/// <returns>A doujin if found</returns>
	[HttpGet("{id:length(24)}")]
	[Produces("application/json")]
	public async Task<ActionResult<Doujin>> Get(string id)
	{
		var doujin = await _doujinService.GetAsyncDocId(id);
		if (doujin == null)
			return new NotFoundResult();
		return doujin;
	}

	/// <summary>
	/// Get a doujin by its doujin id.
	/// </summary>
	/// <param name="id">The doujin id.</param>
	/// <returns>A doujin if found.</returns>
	[HttpGet("doujinId/{id}")]
	[Produces("application/json")]
	public async Task<ActionResult<Doujin>> GetId(string id)
	{
		var doujin = await _doujinService.GetAsyncId(id);
		if (doujin == null)
			return new NotFoundResult();
		return doujin;
	}

	/// <summary>
	/// Get the count of doujins in the database.
	/// </summary>
	/// <returns>The number of doujins in the database.</returns>
	[HttpGet("count")]
	public async Task<int> GetCount() => await _doujinService.GetDoujinsCountAsync();

	/// <summary>
	/// Create a doujin and insert it in the database.
	/// </summary>
	/// <param name="doujin">The new doujin</param>
	/// <returns>201 on success</returns>
	[HttpPost]
	public async Task<IActionResult> Create(Doujin doujin)
	{
		await _doujinService.CreateAsync(doujin);

		return new CreatedResult($"/api/v1/doujins/{doujin.Id}", doujin);
	}

	/// <summary>
	/// Delete a doujin by its document id.
	/// </summary>
	/// <param name="id">The doujin's document id.</param>
	/// <returns>204 on success</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id)
	{
		var doujin = await _doujinService.GetAsyncDocId(id);
		if (doujin == null)
			return new NoContentResult();

		await _doujinService.DeleteAsync(id);

		return new NoContentResult();
	}

	/// <summary>
	///  Update a doujin by its document id. (Complete replacement)
	/// </summary>
	/// <param name="id"> The document id of the doujin </param>
	/// <param name="doujinIn">The updated doujin</param>
	/// <returns></returns>
	[HttpPut("{id:length(24)}")]
	public async Task<IActionResult> Update(string id, Doujin doujinIn)
	{
		var doujin = await _doujinService.GetAsyncDocId(id);
		if (doujin == null)
			return new NotFoundResult();

		await _doujinService.UpdateAsync(doujinIn);
		return new OkResult();
	}
	
	
	[HttpGet("fetch/{url}")]
	[Produces("application/json")]
	public async Task<Doujin> FetchDoujin(string url)
	{
		return await Exhentai.GetDoujinAsync(url);
	}
	
	
	
}


