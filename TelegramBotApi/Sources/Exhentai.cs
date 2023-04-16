using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using TelegramBotApi.Exceptions;
using TelegramBotApi.Models;
using TelegramBotApi.Models.Exhentai;
using TelegramBotApi.Services;

namespace TelegramBotApi.Sources;

/// <summary>
/// The source logic for exhentai/e-hentai.
/// </summary>
public static class Exhentai
{
	/// <summary>
	/// Parse the url to get the gallery ID, gallery token and domain.
	/// </summary>
	/// <param name="url">The url to parse</param>
	/// <returns>The gallery ID, gallery token and domain</returns>
	private static (int, string, string) ParseUrl(string url)
	{
		url = url.Replace("?nw=always", "");

		string[] urlSplit = url.Split('/');
		int galleryId = int.Parse(urlSplit[4]);
		string galleryToken = urlSplit[5];
		string domain = urlSplit[2];
		return (galleryId, galleryToken, domain);
	}

	/// <summary>
	/// Get a doujin from exhentai/e-hentai.
	/// </summary>
	/// <param name="doujinUrl">The doujin's url.</param>
	/// <param name="settingsService">The bot's settings database service.</param>
	/// <param name="doujinService">The doujin's database service.</param>
	/// <returns>A doujin</returns>
	/// <exception cref="ExhentaiException"></exception>
	public static async Task<Doujin> GetDoujinAsync(string doujinUrl, SettingService settingsService,
		DoujinService doujinService)
	{
		var (httpClient, settings) = await GetHttpClientAndSettings(settingsService);


		var urlReg = new Regex(@"^https:\/\/(e-hentai|exhentai)\.org\/g\/");
		if (!urlReg.IsMatch(doujinUrl))
			throw new ExhentaiException("Invalid url", (int) HttpStatusCode.BadRequest,
				ExhentaiExceptionType.InvalidDoujinUrl, "");

		(int id, string token, string domain) = ParseUrl(doujinUrl);

		var doujin = await doujinService.GetAsyncId(id.ToString());

		if (doujin != null)
			return doujin;

		var metadata = await GetMetadata(id, token, domain, httpClient);

		doujin = await ConstructDoujin(metadata, settings, doujinUrl, httpClient);

		await doujinService.CreateAsync(doujin);

		return doujin;
	}

	/// <summary>
	/// Get the metadata of a doujin.
	/// </summary>
	/// <param name="id">The doujin's id.</param>
	/// <param name="token">The doujin's token.</param>
	/// <param name="domain">The doujin's domain.</param>
	/// <param name="client">The HTTP client.</param>
	/// <returns>The medatada of the requested doujin.</returns>
	/// <exception cref="ExhentaiException">If the request is bad.</exception>
	private static async Task<MetadataDoujin> GetMetadata(int id, string token, string domain, HttpClient client)
	{
		var url = $"https://{domain}/api.php";

		var apiRequest = new ApiRequest
		{
			Method = "gdata",
			Namespace = 1,
			Gidlist = new[]
			{
				new[]
				{
					new Gidlist
					{
						Integer = id,
					},
					new Gidlist
					{
						String = token
					}
				},
			}
		};

		var response =
			await client.PostAsync(url, new StringContent(apiRequest.ToJson(), Encoding.UTF8, "application/json"));

		var responseString = await response.Content.ReadAsStringAsync();

		var metadata = MetadataDoujin.FromJson(responseString);

		if (metadata.Gmetadata?[0].Error != null)
			throw new ExhentaiException(metadata.Gmetadata[0].Error, (int) HttpStatusCode.BadRequest,
				ExhentaiExceptionType.BadMetadataRequest, "");

		return metadata;
	}

	/// <summary>
	/// The logic to construct a doujin from the metadata.
	/// </summary>
	/// <param name="metadata">The doujin's metadata</param>
	/// <param name="settings">The bot's settings.</param>
	///  <param name="doujinUrl">The doujin's url.</param>
	///  <param name="httpClient">The HTTP client.</param>
	/// <returns>A doujin.</returns>
	private static async Task<Doujin> ConstructDoujin(MetadataDoujin metadata, Setting settings, string doujinUrl,
		HttpClient httpClient)
	{
		if (metadata.Gmetadata == null)
			throw new ExhentaiException("No metadata found", (int) HttpStatusCode.BadRequest,
				ExhentaiExceptionType.BadMetadataRequest, "");

		var metadataDoujin = metadata.Gmetadata[0];

		var tags = metadataDoujin.Tags.Select(tag =>
		{
			var parts = tag.Split(':');
			var tagValue = parts.Length > 1 ? parts[1] : parts[0];
			// replce spaces and dashes with underscores
			tagValue = tagValue.Replace(' ', '_').Replace('-', '_');
			return $"#{tagValue}";
		}).ToList();

		var imageUrls = await GetImageUrls(doujinUrl, metadataDoujin.Filecount, settings, httpClient);

		return new Doujin
		{
			DoujinId = metadataDoujin.Gid.ToString(),
			Rating = metadataDoujin.Rating,
			Url = doujinUrl,
			FileName = Regex.Replace(metadataDoujin.Title, @"[^a-zA-Z0-9 ]", "").Replace(" ", "_"),
			Posted = metadataDoujin.Posted,
			Category = metadataDoujin.Category,
			Title = metadataDoujin.Title,
			FileCount = (int) metadataDoujin.Filecount,
			Tags = tags,
			ImageUrls = imageUrls,
			Thumbnail = metadataDoujin.Thumb,
			TelegraphUrl = "",
			Source = Source.Exhentai
		};
	}

	/// <summary>
	/// Get the image urls of a doujin.
	/// </summary>
	/// <param name="doujinUrl">The doujin's url</param>
	/// <param name="metadataDoujinFilecount">The number of files in the doujin.</param>
	/// <param name="settings">The bot's settings.</param>
	/// <param name="httpClient">The http client.</param>
	/// <returns></returns>
	private static async Task<List<string>> GetImageUrls(string doujinUrl, long metadataDoujinFilecount, Setting settings,
		HttpClient httpClient)
	{
		var pageUrls = GetPageUrls(doujinUrl, metadataDoujinFilecount, settings.MaxFiles);

		var imageUrls = new List<string>();
		var galleryUrls = new List<string>();

		foreach (string pageUrl in pageUrls)
		{
			var pageGalleryUrls = await GetPageGalleryUrls(pageUrl, httpClient);
			galleryUrls.AddRange(pageGalleryUrls);
		}

		long maxUrls = Math.Min(galleryUrls.Count, settings.MaxFiles);

		var tasks = new List<Task<string>>();
		
		for (int i = 0; i < maxUrls; i++)
		{
			tasks.Add(GetImageUrl(galleryUrls[i], httpClient));
		}
		
		imageUrls.AddRange(await Task.WhenAll(tasks));

		return imageUrls;
	}

	/// <summary>
	/// Get the image src url from a gallery url.
	/// </summary>
	/// <param name="galleryUrl">The gallery url.</param>
	/// <param name="httpClient">The http client.</param>
	/// <returns></returns>
	/// <exception cref="ExhentaiException">If the page fails to load.</exception>
	private static async Task<string> GetImageUrl(string galleryUrl, HttpClient httpClient)
	{
		var html = new HtmlDocument();
		html.LoadHtml(await httpClient.GetStringAsync(galleryUrl));
		if (html.DocumentNode == null)
			throw new ExhentaiException("Error loading gallery url", (int) HttpStatusCode.InternalServerError,
				ExhentaiExceptionType.BadPageRequest, "");

		var imageUrlNode = html.DocumentNode.SelectSingleNode("//*[@id='img']");


		return imageUrlNode.GetAttributeValue("src", "");
	}

	/// <summary>
	/// Get all the gallery urls for a doujin page.
	/// </summary>
	/// <param name="pageUrl">The page url</param>
	/// <param name="httpClient">The http client.</param>
	/// <returns>All the gallery urls for the page.</returns>
	/// <exception cref="ExhentaiException">If the page fails to load.</exception>
	private static async Task<List<string>> GetPageGalleryUrls(string pageUrl, HttpClient httpClient)
	{
		var html = new HtmlDocument();
		html.LoadHtml(await httpClient.GetStringAsync(pageUrl));
		if (html.DocumentNode == null)
			throw new ExhentaiException("Error loading page url", (int) HttpStatusCode.InternalServerError,
				ExhentaiExceptionType.BadPageRequest, "");

		var galleryUrlNodes = html.DocumentNode.SelectNodes("//*[@id='gdt']").Descendants("a");

		return galleryUrlNodes.Select(galleryUrlNode => galleryUrlNode.Attributes["href"].Value).ToList();
	}

	/// <summary>
	/// Calculate the page urls for a doujin.
	/// </summary>
	/// <param name="doujinUrl">The doujin's url</param>
	/// <param name="metadataDoujinFilecount">The doujin's file count</param>
	/// <param name="settingsMaxFiles">The maximum number of allowed files.</param>
	/// <returns>An array with all the page urls.</returns>
	private static IEnumerable<string> GetPageUrls(string doujinUrl, long metadataDoujinFilecount, int settingsMaxFiles)
	{
		long pageCount = metadataDoujinFilecount / 20;

		if (metadataDoujinFilecount % 20 != 0)
			pageCount++;

		if (metadataDoujinFilecount > settingsMaxFiles)
			pageCount = settingsMaxFiles / 20;

		string[] pageUrls = new string[pageCount];


		for (int i = 0; i < pageCount; i++)
		{
			string pageUrl = doujinUrl + $"?p={i}";
			pageUrls[i] = pageUrl;
		}


		return pageUrls;
	}


	/// <summary>
	/// Parse the user given tags.
	/// </summary>
	/// <param name="tags">The tag string</param>
	/// <returns>The tag string to be inserted in the search url, the positive and negative tags for stats</returns>
	public static (string, List<string>, List<string>) ParseTags(string tags)
	{
		if (string.IsNullOrEmpty(tags))
			return ("", new List<string>(), new List<string>());

		var negativeRegex = new Regex(@"\([^)]*\)|\[[^\]]*\]g;", RegexOptions.Multiline);
		var positiveRegex = new Regex(@"^[^\(]+", RegexOptions.Multiline);

		string positiveTagsMatch = positiveRegex.Match(tags).Value;
		string negativeTagsMatch = negativeRegex.Match(tags).Value;

		var positiveTags = new List<string>();
		var negativeTags = new List<string>();

		if (!string.IsNullOrEmpty(negativeTagsMatch))
		{
			negativeTags = Regex.Replace(negativeTagsMatch, @"\(|\)", "").Split("#").ToList()
				.Where(tag => !string.IsNullOrEmpty(tag.Trim())).ToList();
			negativeTags = negativeTags.Select(tag => tag.Trim()).ToList();
		}

		if (!string.IsNullOrEmpty(positiveTagsMatch))
		{
			positiveTags = positiveTagsMatch.Split("#").ToList()
				.Where(tag => !string.IsNullOrEmpty(tag.Trim())).ToList();
			positiveTags = positiveTags.Select(tag => tag.Trim()).ToList();
		}

		string tagsString = positiveTags
			.Select(tag =>
			{
				tag = tag.Replace("_", "+");
				return $"+\"{tag}\"";
			})
			.Concat(negativeTags.Select(tag =>
			{
				tag = tag.Replace("_", "+");
				return $"+-\"{tag}\"";
			}))
			.Aggregate((current, next) => current + next).Replace(" ", "");


		return (tagsString, positiveTags, negativeTags);
	}

	/// <summary>
	/// Get a random doujin url from the search results.
	/// </summary>
	/// <param name="settingsService">The settings database service.</param>
	/// <param name="tagsString">The pre-formated and parsed tags string</param>
	/// <returns>A random doujin url</returns>
	/// <exception cref="ExhentaiException">If no doujins are found.</exception>
	public static async Task<string> GetRandomDoujinUrl(SettingService settingsService, string tagsString)
	{
		var (httpClient, settings) = await GetHttpClientAndSettings(settingsService);
		string searchUrl =
			$"https://exhentai.org/?f_search=language:english{tagsString}&advsearch=1&f_srdd=4&f_spf=1&f_spt={settings.MaxFiles}";
		var pageLimit = await GetPageLimit(searchUrl, httpClient);
		var randomPrevNumber = new Random().Next(0, pageLimit - 1);

		searchUrl += $"&prev={randomPrevNumber}";

		var html = new HtmlDocument();
		html.LoadHtml(await httpClient.GetStringAsync(searchUrl));

		var doujinUrlNodes = html.DocumentNode.SelectNodes("/html/body/div[2]/div[2]/div[4]").Descendants("a");

		var doujinUrls = doujinUrlNodes.Select(doujinUrlNode => doujinUrlNode.Attributes["href"].Value)
			.Where(doujinUrl => doujinUrl.Contains("https://exhentai.org/g/")).ToList();

		string? randomDoujinUrl = doujinUrls[new Random().Next(0, doujinUrls.Count - 1)];


		if (string.IsNullOrEmpty(randomDoujinUrl))
			throw new ExhentaiException("No doujins found.", (int) HttpStatusCode.NotFound,
				ExhentaiExceptionType.NoResults, "");

		return randomDoujinUrl;
	}


	/// <summary>
	/// Get the upper limit of the Gid number for a search (Gid of the first doujin of the first page).
	/// </summary>
	/// <param name="searchUrl">The search url.</param>
	/// <param name="httpClient">The http client</param>
	/// <returns>The first GID </returns>
	/// <exception cref="ExhentaiException">If no doujins are found.</exception>
	private static async Task<int> GetPageLimit(string searchUrl, HttpClient httpClient)
	{
		var html = new HtmlDocument();
		html.LoadHtml(await httpClient.GetStringAsync(searchUrl));

		if (html.DocumentNode == null)
			throw new ExhentaiException("Error loading search url.", (int) HttpStatusCode.InternalServerError,
				ExhentaiExceptionType.BadPageRequest, "");

		var pageLimitNode = html.DocumentNode.SelectSingleNode("/html/body/div[2]/div[2]/div[4]/div[1]/a");

		if (pageLimitNode == null)
			throw new ExhentaiException("No results for those criterias.", (int) HttpStatusCode.NotFound,
				ExhentaiExceptionType.NoResults, "");

		int pageLimit = int.Parse(pageLimitNode.Attributes["href"].Value.Split("/")[4]);

		return pageLimit;
	}


	/// <summary>
	///  Get the http client and the settings.
	/// </summary>
	/// <param name="settingsService">The settings database service.</param>
	/// <returns>The configured Http client and the bot's settings.</returns>
	/// <exception cref="ExhentaiException">If no cookies are in the settings.</exception>
	private static async Task<(HttpClient, Setting)> GetHttpClientAndSettings(SettingService settingsService)
	{
		var settings = await settingsService.GetAsync();

		if (!settings.Cookies.TryGetValue(Source.Exhentai, out _))
			throw new ExhentaiException("No cookies found for exhentai.", (int) HttpStatusCode.BadRequest,
				ExhentaiExceptionType.NoExhentaiCookies, "");
		

		var httpClient = new HttpClient
		{
			DefaultRequestHeaders =
			{
				{
					"User-Agent",
					"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
				},
				{"Cookie", settings.Cookies[Source.Exhentai]}
			},
		};

		return (httpClient, settings);
	}
}