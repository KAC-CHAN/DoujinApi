using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBotApi.Models;

/// <summary>
/// This class represents a setting object.
/// </summary>
[BsonIgnoreExtraElements]
public class Setting
{
	/// <summary>
	/// The document ID.
	/// </summary>
	[BsonId]
	[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
	public string? Id { get; set; }

	/// <summary>
	/// The name of the setting document.
	/// </summary>
	[BsonElement("name")]
	public string Name { get; set; } = "settings";

	/// <summary>
	/// The users whitelist (no daily limits).
	/// </summary>
	[BsonElement("whitelist_users")]
	public List<int> WhitelistUsers { get; set; } = new();

	/// <summary>
	/// The groups that can use the bot.
	/// </summary>
	[BsonElement("whitelist_groups")]
	public List<long> WhitelistGroups { get; set; } = new();

	/// <summary>
	/// The loading messages.
	/// </summary>
	[BsonElement("loading_messages")]
	public List<string> LoadingMessages { get; set; } = new();

	/// <summary>
	/// The loading gifs.
	/// </summary>
	[BsonElement("loading_gifs")]
	public List<string> LoadingGifs { get; set; } = new();

	/// <summary>
	/// The max daily use per non whitelisted user.
	/// </summary>
	[BsonElement("max_daily_use")]
	public int MaxDailyUse { get; set; } = 20;

	/// <summary>
	/// The max files per doujin.
	/// </summary>
	[BsonElement("max_files")]
	public int MaxFiles { get; set; } = 65;

	/// <summary>
	/// The allowed commands for the different groups.
	/// </summary>
	[BsonElement("allowed_commands_groups")]
	public Dictionary<string, string[]> AllowedCommandsGroups { get; set; } = new();

	/// <summary>
	/// The cookies for the different sources.
	/// </summary>
	[BsonElement("cookies")]
	public Dictionary<Source, string> Cookies { get; set; } = new();
}

/// <summary>
/// An enum for the different sources.
/// </summary>
public enum Source
{
	/// <summary>
	/// Exhentai source (also includes e-hentai).
	/// </summary>
	Exhentai = 0,
}