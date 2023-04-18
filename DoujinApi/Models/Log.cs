using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace DoujinApi.Models;

/// <summary>
/// Class that represents a log object.
/// </summary>
[BsonIgnoreExtraElements]
public class Log
{
	/// <summary>
	/// The log's document id.
	/// </summary>
	[BsonId]
	[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
	public string? Id { get; set; }
	/// <summary>
	/// The log's level.
	/// </summary>
	[BsonElement("level")]
	public LogLevel Level { get; set; } = LogLevel.NotSet;
	/// <summary>
	/// The log's message.
	/// </summary>
	[BsonElement("message")]
	public string Message { get; set; } = string.Empty;
	
	/// <summary>
	/// The log's timestamp.
	/// </summary>
	[BsonElement("timestamp")]
	public long Timestamp { get; set; } = 0;

}

/// <summary>
/// Log level.
/// </summary>
public enum LogLevel
{
	/// <summary>
	/// The log level is not set.
	/// </summary>
	[Description ("[NotSet]")]
	NotSet = 0,
	/// <summary>
	/// The log level is set to debug.
	/// </summary>
	[Description ("[Debug]")]
	Debug = 1,
	/// <summary>
	/// The log level is set to info.
	/// </summary>
	[Description("[Info]")]
	Info = 2,
	/// <summary>
	/// The log level is set to warning.
	/// </summary>
	[Description("[Warning]")]
	Warning = 3,
	/// <summary>
	/// The log level is set to error.
	/// </summary>
	[Description("[Error]")]
	Error = 4,
	/// <summary>
	/// The log level is set to critical.
	/// </summary>
	[Description("[Critical]")]
	Critical = 5,
	/// <summary>
	/// The log level is set to none.
	/// </summary>
	[Description("[None]")]
	None = 6
}