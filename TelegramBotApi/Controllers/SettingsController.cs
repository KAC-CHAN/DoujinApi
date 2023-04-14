using Microsoft.AspNetCore.Mvc;
using TelegramBotApi.Models;
using TelegramBotApi.Services;

namespace TelegramBotApi.Controllers;

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
	/// <returns>A settings document.</returns>
	[HttpGet]
	public async Task<Setting> Get() => await _settingsService.GetAsync();

	/// <summary>
	/// Update the settings document.
	/// </summary>
	/// <param name="setting">The settings document to update.</param>
	/// <returns>200 OK on success</returns>
	[HttpPut]
	public async Task<IActionResult> Update(Setting setting)
	{
		await _settingsService.UpdateAsync(setting);
		return new OkResult();
	}

	/// <summary>
	/// Delete the settings document.
	/// </summary>
	/// <param name="id">The settings document ID.</param>
	/// <returns>204 No content on delete</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id)
	{
		await _settingsService.DeleteAsync(id);
		return new NoContentResult();
	}
}