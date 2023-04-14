namespace TelegramBotApi.Models;

/// <summary>
/// The user secret settings for mongoDB.
/// </summary>
public class Env
{
	/// <summary>
	/// The mongoDB connection string.
	/// </summary>
	public string MongoConnectionString { get; set; } = null!;
}