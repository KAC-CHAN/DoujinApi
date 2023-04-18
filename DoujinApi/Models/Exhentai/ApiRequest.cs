using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoujinApi.Models.Exhentai;
/// <summary>
/// The request model for exhentai/e-hentai.
/// </summary>
public class ApiRequest
{
	[JsonProperty("method")]
	public string Method { get; set; }

	[JsonProperty("gidlist")]
	public Gidlist[][] Gidlist { get; set; }

	[JsonProperty("namespace")]
	public long Namespace { get; set; }
}

/// <summary>
/// The gidlist model for exhentai/e-hentai request.
/// </summary>
public struct Gidlist
{
	public long? Integer;
	public string String;

	public static implicit operator Gidlist(long Integer) => new Gidlist { Integer = Integer };
	public static implicit operator Gidlist(string String) => new Gidlist { String = String };
}

public static class Serialize
{
	public static string ToJson(this ApiRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
}
/// <summary>
/// The converter for exhentai/e-hentai request gidlist.
/// </summary>
internal static class Converter
{
	public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
	{
		MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
		DateParseHandling = DateParseHandling.None,
		Converters =
		{
			GidlistConverter.Singleton,
			new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
		},
	};
}

/// <summary>
/// The converter for exhentai/e-hentai request gidlist.
/// </summary>
internal class GidlistConverter : JsonConverter
{
	public override bool CanConvert(Type t) => t == typeof(Gidlist) || t == typeof(Gidlist?);

	public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
	{
		switch (reader.TokenType)
		{
			case JsonToken.Integer:
				var integerValue = serializer.Deserialize<long>(reader);
				return new Gidlist { Integer = integerValue };
			case JsonToken.String:
			case JsonToken.Date:
				var stringValue = serializer.Deserialize<string>(reader);
				return new Gidlist { String = stringValue };
		}
		throw new Exception("Cannot unmarshal type Gidlist");
	}

	public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
	{
		var value = (Gidlist)untypedValue;
		if (value.Integer != null)
		{
			serializer.Serialize(writer, value.Integer.Value);
			return;
		}
		if (value.String != null)
		{
			serializer.Serialize(writer, value.String);
			return;
		}
		throw new Exception("Cannot marshal type Gidlist");
	}

	public static readonly GidlistConverter Singleton = new GidlistConverter();
}