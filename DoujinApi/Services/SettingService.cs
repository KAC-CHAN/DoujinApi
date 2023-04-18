using DoujinApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DoujinApi.Models;
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
	/// <returns>The settings document.</returns>
	public async Task<Setting> GetAsync()
	{
		var settings = await _settings.Find(setting => setting.Name == "settings").FirstOrDefaultAsync();
		if (settings != null) return settings;
		
		settings = new Setting();
		await CreateAsync(settings);
		await _loggerService.Log(LogLevel.Info, "No settings detected, creating settings...");
		
		return settings;
	}

	/// <summary>
	/// Create a setting inside the database.
	/// </summary>
	/// <param name="setting">The new settings document</param>
	private async Task CreateAsync(Setting setting)
	{
		await _settings.InsertOneAsync(setting);
	}
	
	/// <summary>
	/// Delete a setting document from the database.
	/// </summary>
	/// <param name="id">Setting's document ID</param>
	public async Task DeleteAsync(string id)
	{
		await _settings.DeleteOneAsync(setting => setting.Id == id);
	}
	
	/// <summary>
	/// Update the settings document.
	/// </summary>
	/// <param name="setting">The updated settings document.</param>
	public async Task UpdateAsync(Setting setting)
	{
		await _settings.ReplaceOneAsync(s => s.Id == setting.Id, setting);
	}
	
}