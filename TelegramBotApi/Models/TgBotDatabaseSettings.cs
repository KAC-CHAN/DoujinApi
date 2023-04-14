namespace TelegramBotApi.Models;

/// <summary>
/// The settings for the database.
/// </summary>
public class TgBotDatabaseSettings
{
	/// <summary>
	/// The connection string to the database
	/// </summary>
	public string ConnectionString { get; set; } = null!;
	/// <summary>
	/// The name of the database
	/// </summary>
	public string DatabaseName { get; set; } = null!;
	/// <summary>
	/// The name of the doujins collection
	/// </summary>
	public string DoujinsCollectionName { get; set; } = null!;
	/// <summary>
	/// The name of the users collection
	/// </summary>
	public string UsersCollectionName { get; set; } = null!;
	/// <summary>
	/// The name of the logs collection
	/// </summary>
	public string LogsCollectionName { get; set; } = null!;
	/// <summary>
	/// The name of the settings collection
	/// </summary>
	public string SettingsCollectionName { get; set; } = null!;
	
	/// <summary>
	/// The name of the stats collection
	/// </summary>
	public string StatsCollectionName { get; set; } = null!;
}