using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoujinApi.Models.Exhentai;

/// <summary>
///	This class represents a doujin metadata object.
/// </summary>
public partial class MetadataDoujin
{
	/// <summary>
	/// The Metadata body.
	/// </summary>
	[JsonProperty("gmetadata")] public Gmetadatum[]? Gmetadata { get; set; }
}

/// <summary>
/// This is a partial class for MetadataDoujin.
/// </summary>
public class Gmetadatum
{
	/// <summary>
	/// The gallery ID.
	/// </summary>
	[JsonProperty("gid")]
	public long Gid { get; set; }

	/// <summary>
	/// Error message.
	/// </summary>
	[JsonProperty("error")]
	public string? Error { get; set; }
	
	/// <summary>
	/// The gallery token.
	/// </summary>
	[JsonProperty("token")]
	public string Token { get; set; }

	/// <summary>
	/// The gallery archiver key.
	/// </summary>
	[JsonProperty("archiver_key")]
	public string ArchiverKey { get; set; }

	/// <summary>
	/// The gallery title.
	/// </summary>
	[JsonProperty("title")]
	public string Title { get; set; }

	/// <summary>
	/// The gallery title in Japanese.
	/// </summary>
	[JsonProperty("title_jpn")]
	public string TitleJpn { get; set; }

	/// <summary>
	/// The gallery category.
	/// </summary>
	[JsonProperty("category")]
	public string Category { get; set; }

	/// <summary>
	///	The gallery's thumbnail.
	/// </summary>
	[JsonProperty("thumb")]
	public string Thumb { get; set; }

	/// <summary>
	/// The gallery's uploader.
	/// </summary>
	[JsonProperty("uploader")]
	public string Uploader { get; set; }

	/// <summary>
	/// The gallery's posted date (in Unix time).
	/// </summary>
	[JsonProperty("posted")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long Posted { get; set; }
	/// <summary>
	/// The gallery's file count.
	/// </summary>
	[JsonProperty("filecount")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long Filecount { get; set; }
	/// <summary>
	/// The gallery's file size.
	/// </summary>
	[JsonProperty("filesize")] public long Filesize { get; set; }
/// <summary>
/// The gallery's expunged status.
/// </summary>
	[JsonProperty("expunged")] public bool Expunged { get; set; }
/// <summary>
/// The gallery's rating.
/// </summary>
	[JsonProperty("rating")] public string Rating { get; set; }
/// <summary>
/// The gallery's torrent count.
/// </summary>
	[JsonProperty("torrentcount")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long Torrentcount { get; set; }
/// <summary>
/// The gallery's torrens.
/// </summary>
	[JsonProperty("torrents")] public Torrent[]? Torrents { get; set; }
/// <summary>
/// The gallery's tags.
/// </summary>
	[JsonProperty("tags")] public string[] Tags { get; set; }
/// <summary>
/// The gallery's parent gallery ID.
/// </summary>
	[JsonProperty("parent_gid")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long ParentGid { get; set; }
/// <summary>
/// The gallery's parent gallery key.
/// </summary>
	[JsonProperty("parent_key")] public string ParentKey { get; set; }
/// <summary>
/// The gallery's child gallery ID.
/// </summary>
	[JsonProperty("first_gid")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long FirstGid { get; set; }
/// <summary>
/// The gallery's child gallery key.
/// </summary>
	[JsonProperty("first_key")] public string FirstKey { get; set; }
}
/// <summary>
/// This class represents a torrent object.
/// </summary>
public class Torrent
{/// <summary>
 /// The torrent's hash.
 /// </summary>
	[JsonProperty("hash")] public string Hash { get; set; }
/// <summary>
/// The date the torrent was added (in Unix time).
/// </summary>
	[JsonProperty("added")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long Added { get; set; }
/// <summary>
/// The torrent's name.
/// </summary>
	[JsonProperty("name")] public string Name { get; set; }
/// <summary>
/// The torrent's size.
/// </summary>
	[JsonProperty("tsize")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long Tsize { get; set; }
/// <summary>
/// The torrent's file size.
/// </summary>
	[JsonProperty("fsize")]
	[JsonConverter(typeof(ParseStringConverter))]
	public long Fsize { get; set; }
}

public partial class MetadataDoujin
{
	public static MetadataDoujin FromJson(string json) =>
		JsonConvert.DeserializeObject<MetadataDoujin>(json, DoujinMetadataConverter.Settings);
}

internal static class DoujinMetadataConverter
{
	public static readonly JsonSerializerSettings Settings = new ()
	{
		MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
		DateParseHandling = DateParseHandling.None,
		Converters =
		{
			new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
		},
	};
}

internal class ParseStringConverter : JsonConverter
{
	public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

	public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null) return null;
		var value = serializer.Deserialize<string>(reader);
		long l;
		if (Int64.TryParse(value, out l))
		{
			return l;
		}

		throw new Exception("Cannot unmarshal type long");
	}

	public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
	{
		if (untypedValue == null)
		{
			serializer.Serialize(writer, null);
			return;
		}

		var value = (long) untypedValue;
		serializer.Serialize(writer, value.ToString());
		return;
	}

	public static readonly ParseStringConverter Singleton = new ParseStringConverter();
}