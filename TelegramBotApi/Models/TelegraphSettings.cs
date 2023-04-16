namespace TelegramBotApi.Models;
/// <summary>
/// This class represents the settings for telegraph
/// </summary>
public class TelegraphSettings
{
	/// <summary>
	/// Telegraph access token
	/// </summary>
	public string AccessToken { get; set; } = null!;
	/// <summary>
	/// Telegraph author name
	/// </summary>
	public string AuthorName { get; set; } = null!;
	/// <summary>
	/// Telegraph author url
	/// </summary>
	public string AuthorUrl { get; set; } = null!;
}