using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DoujinApi.Models;

/// <summary>
/// This class represents a user object.
/// </summary>
[BsonIgnoreExtraElements]
public class User
{
	/// <summary>
	/// The user's document id.
	/// </summary>
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }

	/// <summary>
	///  The user's Telegram ID.
	/// </summary>
	[BsonElement("user_id")]
	public Int64 UserId { get; set; } = 0;
	

	/// <summary>
	/// The user's username.
	/// </summary>
	[BsonElement("username")]
	public string? Username { get; set; } = string.Empty;

	/// <summary>
	/// The user's first and last name.
	/// </summary>
	[BsonElement("name")]
	public Name Name { get; set; } = new();

	/// <summary>
	/// The user's favorites.
	/// </summary>
	[BsonElement("favorites")]
	public List<int> Favorites { get; set; } = new();

	/// <summary>
	/// The user's doujins history.
	/// </summary>
	[BsonElement("doujins")]
	public List<int> Doujins { get; set; } = new();

	/// <summary>
	/// The user's total usage.
	/// </summary>
	[BsonElement("usage")]
	public int Usage { get; set; } = 0;

	/// <summary>
	/// The user's daily usage.
	/// </summary>
	[BsonElement("daily_use")]
	public int DailyUse { get; set; } = 0;

	/// <summary>
	/// The user's daily usage date.
	/// </summary>
	[BsonElement("daily_use_date")]
	public DateTime DailyUseDate { get; set; } = DateTime.Now;
	
}

/// <summary>
/// This class represents a user's name
/// </summary>
public class Name
{
	/// <summary>
	/// The user's first name
	/// </summary>
	[BsonElement("first")]
	public string? First { get; set; } = string.Empty;

	/// <summary>
	/// The user's last name
	/// </summary>
	[BsonElement("last")]
	public string? Last { get; set; } = string.Empty;
}