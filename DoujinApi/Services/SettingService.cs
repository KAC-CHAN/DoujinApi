using DoujinApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using LogLevel = DoujinApi.Models.LogLevel;

namespace DoujinApi.Services;

/// <summary>
/// The service for the settings.
/// </summary>
public class SettingService
{
	private readonly IMongoCollection<Setting> _settings;
	private readonly LoggerService _loggerService;

	/// <summary>
	/// The constructor for the setting service.
	/// </summary>
	/// <param name="settings">The database settings</param>
	/// <param name="loggerService">The logger service</param>
	public SettingService(IOptions<DatabaseSettings> settings, LoggerService loggerService)
	{
		var client = new MongoClient(settings.Value.ConnectionString);
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_settings = database.GetCollection<Setting>(settings.Value.SettingsCollectionName);
		_loggerService = loggerService;
	}

	/// <summary>
	/// Returns the settings from the database or creates a new one if none are found.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The settings document.</returns>
	public async Task<Setting> GetAsync(CancellationToken ct)
	{
		var settings = await _settings.Find(setting => setting.Name == "settings").FirstOrDefaultAsync(cancellationToken: ct);
		if (settings != null) return settings;
		
		settings = new Setting();
		await CreateAsync(settings, ct);
		await _loggerService.Log(LogLevel.Info, "No settings detected, creating settings...", ct);
		
		return settings;
	}

	/// <summary>
	/// Create a setting inside the database.
	/// </summary>
	/// <param name="setting">The new settings document</param>
	/// <param name="ct">Cancellation token</param>
	private async Task CreateAsync(Setting setting, CancellationToken ct)
	{
		await _settings.InsertOneAsync(setting, cancellationToken: ct);
	}
	
	/// <summary>
	/// Delete a setting document from the database.
	/// </summary>
	/// <param name="id">Setting's document ID</param>
	/// <param name="ct">Cancellation token</param>
	public async Task DeleteAsync(string id, CancellationToken ct)
	{
		await _settings.DeleteOneAsync(setting => setting.Id == id, cancellationToken: ct);
	}
	
	/// <summary>
	/// Update the settings document.
	/// </summary>
	/// <param name="setting">The updated settings document.</param>
	/// <param name="ct">Cancellation token</param>
	public async Task UpdateAsync(Setting setting, CancellationToken ct)
	{
		await _settings.ReplaceOneAsync(s => s.Id == setting.Id, setting, cancellationToken: ct);
	}
	
}