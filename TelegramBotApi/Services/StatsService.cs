using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TelegramBotApi.Models;
using LogLevel = TelegramBotApi.Models.LogLevel;

namespace TelegramBotApi.Services;
/// <summary>
/// The stats service.
/// </summary>
public class StatsService
{
	private readonly IMongoCollection<Stats> _stats;
	private readonly LoggerService _loggerService;
	
	/// <summary>
	/// The constructor for the stats service.
	/// </summary>
	/// <param name="settings">The database settings</param>
	/// <param name="loggerService">Logging service</param>
	public StatsService(IOptions<TgBotDatabaseSettings> settings, LoggerService loggerService)
	{
		var client = new MongoClient(settings.Value.ConnectionString);;
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_stats = database.GetCollection<Stats>(settings.Value.StatsCollectionName);
		_loggerService = loggerService;
	}
	
	/// <summary>
	/// Get the stats from the database. If no stats are found, create a new one .
	/// </summary>
	/// <returns>The stats document.</returns>
	public async Task<Stats> GetAsync()
	{
		var stats = await _stats.Find(stat => stat.Name == "stats").FirstOrDefaultAsync();
		if (stats != null) return stats;
		
		stats = new Stats();
		await CreateAsync(stats);
		await _loggerService.Log(LogLevel.Info, "No stats detected, creating stats...");
		return stats;
	}
	
	/// <summary>
	/// Create a stats document inside the database.
	/// </summary>
	/// <param name="stats">The new stats document.</param>
	private async Task CreateAsync(Stats stats)
	{
		await _stats.InsertOneAsync(stats);
	}
	/// <summary>
	/// Update a stats document in the database.
	/// </summary>
	/// <param name="stats">The updated stats document.</param>
	public async Task UpdateAsync(Stats stats)
	{
		await _stats.ReplaceOneAsync(stat => stat.Id == stats.Id, stats);
	}
	
	/// <summary>
	/// Delete a stats document from the database.
	/// </summary>
	/// <param name="id">The stats's document ID.</param>
	public async Task DeleteAsync(string id)
	{
		await _stats.DeleteOneAsync(stat => stat.Id == id);
	}
	
	
	
	
	
	
	
	
}