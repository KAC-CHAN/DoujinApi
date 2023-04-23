using DoujinApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DoujinApi.Models;
using LogLevel = DoujinApi.Models.LogLevel;

namespace DoujinApi.Services;
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
	public StatsService(IOptions<DatabaseSettings> settings, LoggerService loggerService)
	{
		var client = new MongoClient(settings.Value.ConnectionString);;
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_stats = database.GetCollection<Stats>(settings.Value.StatsCollectionName);
		_loggerService = loggerService;
	}
	
	/// <summary>
	/// Get the stats from the database. If no stats are found, create a new one .
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The stats document.</returns>
	public async Task<Stats> GetAsync(CancellationToken ct)
	{
		var stats = await _stats.Find(stat => stat.Name == "stats").FirstOrDefaultAsync(cancellationToken: ct);
		if (stats != null) return stats;
		
		stats = new Stats();
		await CreateAsync(stats);
		await _loggerService.Log(LogLevel.Info, "No stats detected, creating stats...", ct);
		return stats;
	}
	
	/// <summary>
	/// Create a stats document inside the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <param name="stats">The new stats document.</param>
	private async Task CreateAsync(Stats stats, CancellationToken ct = default)
	{
		await _stats.InsertOneAsync(stats, cancellationToken: ct);
	}
	/// <summary>
	/// Update a stats document in the database.
	/// </summary>
	/// <param name="stats">The updated stats document.</param>
	/// <param name="ct">Cancellation token</param>
	public async Task UpdateAsync(Stats stats, CancellationToken ct )
	{
		await _stats.ReplaceOneAsync(stat => stat.Id == stats.Id, stats, cancellationToken: ct);
	}
	
	/// <summary>
	/// Delete a stats document from the database.
	/// </summary>
	/// <param name="id">The stats's document ID.</param>
	/// <param name="ct">Cancellation token</param>
	public async Task DeleteAsync(string id, CancellationToken ct)
	{
		await _stats.DeleteOneAsync(stat => stat.Id == id, cancellationToken: ct);
	}
	
	
	
	
	
	
	
	
}