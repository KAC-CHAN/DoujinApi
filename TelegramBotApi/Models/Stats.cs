using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBotApi.Models;

/// <summary>
/// This class represents a stats object
/// </summary>
public class Stats
{
	/// <summary>
	/// The stats document ID.
	/// </summary>
	[BsonId]
	[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
	public string? Id { get; set; }

	/// <summary>
	/// The name of the stats document.
	/// </summary>
	[BsonElement("name")] public string Name { get; set; } = "stats";

	/// <summary>
	///	The tags used overall.
	/// </summary>
	[BsonElement("tags")]
	public Tags Tags { get; set; } = new();
	
	/// <summary>
	/// The number of times the bot was used per source.
	/// </summary>
	[BsonElement("use_per_source")]
	public Dictionary<Source, int> UsePerSource { get; set; } = new();
	
	/// <summary>
	/// The total number of times the bot was used.
	/// </summary>
	[BsonElement("total_use")]
	public int TotalUse { get; set; } = 0;
	
	/// <summary>
	/// The number of times the /random command was used.
	/// </summary>
	[BsonElement("random_use")]
	public int RandomUse { get; set; } = 0;
	
	/// <summary>
	/// The number of times the /fetch command was used.
	/// </summary>
	[BsonElement("fetch_use")]
	public int FetchUse { get; set; } = 0;

}

/// <summary>
/// This class represents a tag object
/// </summary>
public class Tags
{
	/// <summary>
	/// The positive tags.
	/// </summary>
	[BsonElement("positive")]
	public Dictionary<string, int> Positive { get; set; } = new();

	/// <summary>
	/// The negative tags.
	/// </summary>
	[BsonElement("negative")]
	public Dictionary<string, int> Negative { get; set; } = new();
}