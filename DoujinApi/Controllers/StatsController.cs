using DoujinApi.Models;
using DoujinApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoujinApi.Controllers;

/// <summary>
/// The controller for the stats.
/// </summary>
[ApiController]
[Route("/api/v1/[controller]")]
public class StatsController
{
	private readonly StatsService _statsService;
	
	/// <summary>
	/// The constructor for the stats controller.
	/// </summary>
	/// <param name="statsService">The stats service.</param>
	public StatsController(StatsService statsService)
	{
		_statsService = statsService;
	}
	
	/// <summary>
	/// Get the stats.
	/// </summary>
	/// <returns>The stats document.</returns>
	[HttpGet]
	public async Task<Stats> Get()
	{
		var stats = await _statsService.GetAsync();
		return stats;
	}
	
	/// <summary>
	/// Update the stats.
	/// </summary>
	/// <param name="stats">The updated stats.</param>
	/// <returns>200 OK on sucess.</returns>
	[HttpPut]
	public async Task<IActionResult> Update(Stats stats)
	{
		await _statsService.UpdateAsync(stats);
		return new OkResult();
	}	
	
	/// <summary>
	/// Delete the stats.
	/// </summary>
	/// <param name="id">The stats's document ID.</param>
	/// <returns></returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id)
	{
		var stats = await _statsService.GetAsync();
		if (stats == null)
		{
			return new NotFoundResult();
		}
		await _statsService.DeleteAsync(stats.Id);
		return new NoContentResult();
	}
	
	
	
}