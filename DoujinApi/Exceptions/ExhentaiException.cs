using System.ComponentModel;

namespace DoujinApi.Exceptions;

/// <summary>
/// This class represents an exception thrown by the Exhentai source.
/// </summary>
public class ExhentaiException : Exception
{
	/// <summary>
	/// What HTTP code should be returned.
	/// </summary>
	public int HttpCode { get; set; }
	
	/// <summary>
	/// Name of the Exception.
	/// </summary>
	public ExhentaiExceptionType Name { get; set; }
	
	/// <summary>
	/// Doujin ID if applicable.
	/// </summary>
	public string? DoujinId { get; set; }
	/// <summary>
	/// ExhentaiException constructor.
	/// </summary>
	/// <param name="message">The message to return to the client</param>
	/// <param name="httpCode">The HTTP Code applicable to the exception</param>
	/// <param name="name">The name of the exception</param>
	/// <param name="doujinId">The doujin ID if applicable</param>
	public ExhentaiException(string message, int httpCode,ExhentaiExceptionType name,string doujinId) : base(message)
	{
		HttpCode = httpCode;
		Name = name;
		DoujinId = doujinId;
	}
	
}

/// <summary>
/// This enum represents the name of an ExhentaiException.
/// </summary>
public enum ExhentaiExceptionType
{
	/// <summary>
	/// If the Metadata Request failed.
	/// </summary>
	[Description("Bad Metadata Request")]
	BadMetadataRequest,
	/// <summary>
	/// If the image URL is invalid.
	/// </summary>
	[Description("Bad Image URL")]
	BadImageUrl,
	/// <summary>
	/// If the doujin URL is invalid.
	/// </summary>
	[Description("Bad Doujin URL")]
	InvalidDoujinUrl,
	/// <summary>
	/// There is no Exhentai cookies.
	/// </summary>
	[Description("No Exhentai Cookies")]
	NoExhentaiCookies,
	/// <summary>
	/// The requested page is invalid/failed to load.
	/// </summary>
	[Description("Bad Page Error")]
	BadPageRequest,
	/// <summary>
	/// No results were found for the search.
	/// </summary>
	[Description("No Results")]
	NoResults,
	
	
	
	
}