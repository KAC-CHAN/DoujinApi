using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramBotApi.Models;

/// <summary>
/// This class represents a doujin object
/// </summary>
[BsonIgnoreExtraElements]
public class Doujin
{
	/// <summary>
	/// The doujin's document id.
	/// </summary>
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }

	/// <summary>
	/// The doujin's ID
	/// </summary>
	[BsonElement("doujin_id")]
	public string DoujinId { get; set; } = string.Empty;

	/// <summary>
	/// The doujin's title (cleaned).
	/// </summary>
	[BsonElement("title")]
	public string Title { get; set; } = string.Empty;

	/// <summary>
	/// The doujin's rating.
	/// </summary>
	[BsonElement("rating")]
	public string Rating { get; set; } = string.Empty;

	/// <summary>
	/// The doujin's original Url.
	/// </summary>
	[BsonElement("url")]
	public string Url { get; set; } = string.Empty;

	/// <summary>
	/// The doujin's posted date (Unix timestamp).
	/// </summary>
	[BsonElement("posted")]
	public long Posted { get; set; } = 0;

	/// <summary>
	/// The doujin's category.
	/// </summary>
	[BsonElement("category")]
	public string Category { get; set; } = string.Empty;

	/// <summary>
	/// The doujin's file count / pages count.
	/// </summary>
	[BsonElement("file_count")]
	public int FileCount { get; set; } = 0;

	/// <summary>
	/// The title rendered safe for folder/zip archives names.
	/// </summary>
	[BsonElement("file_name")]
	public string FileName { get; set; } = string.Empty;

	/// <summary>
	/// The doujin's tags list.
	/// </summary>
	[BsonElement("tags")]
	public List<string> Tags { get; set; } = new List<string>();

	/// <summary>
	/// The doujin's thumbnail (First image url or First Telegraph Image url).
	/// </summary>
	[BsonElement("thumbnail")]
	public string Thumbnail { get; set; } = string.Empty;

	/// <summary>
	/// The telegraph url for the doujin.
	/// </summary>
	[BsonElement("telegraph_url")]
	public string? TelegraphUrl { get; set; }

	/// <summary>
	/// The image urls for the doujin.
	/// </summary>
	[BsonElement("image_urls")]
	public List<string> ImageUrls { get; set; } = new List<string>();

	/// <summary>
	/// Where the doujin was found.
	/// </summary>
	[BsonElement("source")]
	public Source Source { get; set; } = new();
}