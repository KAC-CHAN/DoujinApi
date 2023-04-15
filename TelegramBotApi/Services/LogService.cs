using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TelegramBotApi.Models;

namespace TelegramBotApi.Services;

/// <summary>
/// The service for the logs.
/// </summary>
public class LogService
{
	private readonly IMongoCollection<Log> _logs;


	/// <summary>
	/// The constructor for the log service.
	/// </summary>
	/// <param name="settings"></param>
	public LogService(IOptions<TgBotDatabaseSettings> settings)
	{
		var client = new MongoClient(settings.Value.ConnectionString);
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_logs = database.GetCollection<Log>(settings.Value.LogsCollectionName);
	}

	/// <summary>
	/// Get all the logs from the database.
	/// </summary>
	/// <returns>A list of all the logs.</returns>
	public async Task<List<Log>> GetAsync()
	{
		return await _logs.Find(log => true).ToListAsync();
	}

	/// <summary>
	/// Create a log inside the database.
	/// </summary>
	/// <param name="log">The new log to create</param>
	public async Task CreateAsync(Log log)
	{
		await _logs.InsertOneAsync(log);
	}
	/// <summary>
	/// Get a log by its document ID.
	/// </summary>
	/// <param name="id">The log's document ID.</param>
	/// <returns>A log</returns>
	public async Task<Log> GetAsyncById(string id)
	{
		return await _logs.Find(log => log.Id == id).FirstOrDefaultAsync();
	}

	/// <summary>
	/// Delete a log from the database.
	/// </summary>
	/// <param name="id">The log's document ID</param>
	public async Task DeleteAsync(string id)
	{
		await _logs.DeleteOneAsync(log => log.Id == id);
	}

	/// <summary>
	/// Delete all the logs from the database.
	/// </summary>
	public async Task DeleteAllAsync()
	{
		await _logs.DeleteManyAsync(log => true);
	}

	/// <summary>
	/// Get the count of all the logs inside the database.
	/// </summary>
	/// <returns>The number of logs</returns>
	public async Task<int> GetCountAsync()
	{
		return (int) await _logs.CountDocumentsAsync(user => true);
	}
}