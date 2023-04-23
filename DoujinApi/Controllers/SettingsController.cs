using DoujinApi.Models;
using DoujinApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoujinApi.Controllers;

/// <summary>
/// The controller for the settings collection.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class SettingsController
{
	private readonly SettingService _settingsService;

	/// <summary>
	/// The constructor for the settings controller.
	/// </summary>
	/// <param name="settingsService">The setting service instance.</param>
	public SettingsController(SettingService settingsService)
	{
		_settingsService = settingsService;
	}

	/// <summary>
	/// Get the settings from the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A settings document.</returns>
	[HttpGet]
	public async Task<Setting> Get(CancellationToken ct) => await _settingsService.GetAsync(ct);

	/// <summary>
	/// Update the settings document.
	/// </summary>
	/// <param name="setting">The settings document to update.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>200 OK on success</returns>
	[HttpPut]
	public async Task<IActionResult> Update(Setting setting, CancellationToken ct)
	{
		await _settingsService.UpdateAsync(setting, ct);
		return new OkResult();
	}

	/// <summary>
	/// Delete the settings document.
	/// </summary>
	/// <param name="id">The settings document ID.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>204 No content on delete</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id, CancellationToken ct)
	{
		await _settingsService.DeleteAsync(id, ct);
		return new NoContentResult();
	}
}