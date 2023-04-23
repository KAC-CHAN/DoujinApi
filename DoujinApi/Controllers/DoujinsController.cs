using System.Web;
using DoujinApi.Models;
using DoujinApi.Services;
using DoujinApi.Sources;
using DoujinApi.Utils;
using Microsoft.AspNetCore.Mvc;
using DoujinApi.Models;

namespace DoujinApi.Controllers;

/// <summary>
/// Controller for the doujins collection.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class DoujinsController
{
	private readonly DoujinService _doujinService;
	private readonly SettingService _settingService;
	private readonly StatsService _statsService;
	private readonly TelegraphService _telegraphService;

	/// <summary>
	///  Constructor for the doujins controller.
	/// </summary>
	/// <param name="doujinService">The DoujinService Object.</param>
	/// <param name="settingService">The SettingService Object.</param>
	/// <param name="statsService">The StatsService Object.</param>
	/// <param name="telegraphService">The TelegraphService Object.</param>
	public DoujinsController(DoujinService doujinService, SettingService settingService,
		StatsService statsService, TelegraphService telegraphService)
	{
		_doujinService = doujinService;
		_settingService = settingService;
		_statsService = statsService;
		_telegraphService = telegraphService;
	}

	/// <summary>
	/// Get all the doujins inside the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>All the doujins.</returns>
	[HttpGet]
	[Produces("application/json")]
	public async Task<List<Doujin>> Get(CancellationToken ct) => await _doujinService.GetAsync(ct);

	/// <summary>
	/// Get a doujin by its document id.
	/// </summary>
	/// <param name="id">The document id.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A doujin if found</returns>
	[HttpGet("{id:length(24)}")]
	[Produces("application/json")]
	public async Task<ActionResult<Doujin>> Get(string id, CancellationToken ct)
	{
		var doujin = await _doujinService.GetAsyncDocId(id, ct);
		if (doujin == null)
			return new NotFoundResult();
		return doujin;
	}

	/// <summary>
	/// Get a doujin by its doujin id.
	/// </summary>
	/// <param name="id">The doujin id.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A doujin if found.</returns>
	[HttpGet("doujinId/{id}")]
	[Produces("application/json")]
	public async Task<ActionResult<Doujin>> GetId(string id, CancellationToken ct)
	{
		var doujin = await _doujinService.GetAsyncId(id, ct);
		if (doujin == null)
			return new NotFoundResult();
		return doujin;
	}

	/// <summary>
	/// Get the count of doujins in the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The number of doujins in the database.</returns>
	[HttpGet("count")]
	public async Task<int> GetCount(CancellationToken ct) => await _doujinService.GetDoujinsCountAsync(ct);

	/// <summary>
	/// Create a doujin and insert it in the database.
	/// </summary>
	/// <param name="doujin">The new doujin</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>201 on success</returns>
	[HttpPost]
	public async Task<IActionResult> Create(Doujin doujin, CancellationToken ct)
	{
		await _doujinService.CreateAsync(doujin, ct);

		return new CreatedResult($"/api/v1/doujins/{doujin.Id}", doujin);
	}

	/// <summary>
	/// Delete a doujin by its document id.
	/// </summary>
	/// <param name="id">The doujin's document id.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>204 on success</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id, CancellationToken ct)
	{
		var doujin = await _doujinService.GetAsyncDocId(id, ct);
		if (doujin == null)
			return new NoContentResult();

		await _doujinService.DeleteAsync(id, ct);

		return new NoContentResult();
	}

	/// <summary>
	///  Update a doujin by its document id. (Complete replacement)
	/// </summary>
	/// <param name="id"> The document id of the doujin </param>
	/// <param name="doujinIn">The updated doujin</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns></returns>
	[HttpPut("{id:length(24)}")]
	public async Task<IActionResult> Update(string id, Doujin doujinIn, CancellationToken ct)
	{
		var doujin = await _doujinService.GetAsyncDocId(id, ct);
		if (doujin == null)
			return new NotFoundResult();

		await _doujinService.UpdateAsync(doujinIn, ct);
		return new OkResult();
	}

	/// <summary>
	/// Get a doujin by its url and post it to telegraph.
	/// </summary>
	/// <param name="url">The doujin's url</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A doujin.</returns>
	[HttpPost("fetch/")]
	[Produces("application/json")]
	public async Task<ActionResult> FetchDoujin([FromBody] string url, CancellationToken ct)
	{
		string decodedUrl = HttpUtility.UrlDecode(url);
		var doujin = await Exhentai.GetDoujinAsync(decodedUrl, _settingService, _doujinService, ct);
		if (doujin.TelegraphUrl != "") // Doujin already posted send it back with a 200
			return new OkObjectResult(doujin);

		var stats = await _statsService.GetAsync(ct);
		stats.FetchUse++;
		stats.TotalUse++;
		doujin = await _telegraphService.CreatePageAsync(doujin, ct);
		await _doujinService.UpdateAsync(doujin, ct);
		DoujinUtils.CleanUpWorkDirs();
		return new CreatedResult($"/api/v1/doujins/{doujin.Id}", doujin);
	}

	/// <summary>
	/// Get a random doujin with or without tags and post it to telegraph.
	/// </summary>
	/// <param name="tags">The optional tags for the search.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A random doujin.</returns>
	[HttpPost("random/")]
	[Produces("application/json")]
	public async Task<ActionResult> GetRandomDoujin([FromBody] string? tags, CancellationToken ct)
	{
		tags ??= "";

		(string tagsString, var positiveTags, var negativeTags) = Exhentai.ParseTags(tags);
		string randomDoujinUrl = await Exhentai.GetRandomDoujinUrl(_settingService, tagsString, ct);
		var doujin = await Exhentai.GetDoujinAsync(randomDoujinUrl, _settingService, _doujinService, ct);
		if (doujin.TelegraphUrl != "") // Doujin already posted send it back with a 200
			return new OkObjectResult(doujin);
		var stats = await _statsService.GetAsync(ct);
		stats.RandomUse++;
		stats.TotalUse++;
		if (tags != "")
		{
			foreach (string tag in positiveTags)
			{
				if (stats.Tags.Positive.ContainsKey(tag))
					stats.Tags.Positive[tag]++;
				else
					stats.Tags.Positive.Add(tag, 1);
			}

			foreach (string tag in negativeTags)
			{
				if (stats.Tags.Negative.ContainsKey(tag))
					stats.Tags.Negative[tag]++;
				else
					stats.Tags.Negative.Add(tag, 1);
			}
		}

		await _statsService.UpdateAsync(stats, ct);
		doujin = await _telegraphService.CreatePageAsync(doujin, ct);
		await _doujinService.UpdateAsync(doujin, ct);
		DoujinUtils.CleanUpWorkDirs();
		return new CreatedResult($"/api/v1/doujins/{doujin.Id}", doujin);
	}

	/// <summary>
	///		Zips a doujin by its document id. 
	/// </summary>
	/// <param name="id">The doujin's document ID</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns></returns>
	[HttpGet("zip/{id:length(24)}")]
	public async Task<IActionResult> ZipDoujin(string id, CancellationToken ct)
	{
		var doujin = await _doujinService.GetAsyncDocId(id, ct);
		if (doujin == null)
			return new NotFoundResult();

		var stats = await _statsService.GetAsync(ct);
		stats.TotalUse++;
		await _statsService.UpdateAsync(stats, ct);
		string zipDoujin = await DoujinUtils.ZipDoujin(doujin,ct);

		byte[] zip = await File.ReadAllBytesAsync(zipDoujin, ct);

		DoujinUtils.CleanUpWorkDirs();
		return new FileContentResult(zip, "application/zip")
		{
			FileDownloadName = $"{doujin.DoujinId}.zip"
		};
	}

	/// <summary>
	/// Get the number of views for a given Telegraph URL.
	/// </summary>
	/// <param name="url">The telegraph url.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The number of views.</returns>
	[HttpGet("views/{url}")]
	public async Task<ActionResult<int>> GetTpViews(string url, CancellationToken ct)
	{
		var views = await _telegraphService.GetPageViews(HttpUtility.UrlDecode(url));

		return views;
	}
}