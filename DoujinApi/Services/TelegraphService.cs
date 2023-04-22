using DoujinApi.Models;
using DoujinApi.Utils;
using Kvyk.Telegraph;
using Kvyk.Telegraph.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DoujinApi.Models;
using Newtonsoft.Json;
using LogLevel = DoujinApi.Models.LogLevel;

namespace DoujinApi.Services;

/// <summary>
/// Telegraph service.
/// </summary>
public class TelegraphService
{
	private readonly TelegraphClient _telegraphClient;
	private readonly string _authorName;
	private readonly string _authorUrl;
	private readonly LoggerService _loggerService;

	/// <summary>
	/// The telegraph service constructor
	/// </summary>
	/// <param name="settings">The telegraph settings.</param>
	///  <param name="loggerService">The logger service.</param>
	public TelegraphService(IOptions<TelegraphSettings> settings, LoggerService loggerService)
	{
		_telegraphClient = new TelegraphClient
		{
			AccessToken = settings.Value.AccessToken,
		};
		_authorName = settings.Value.AuthorName;
		_authorUrl = settings.Value.AuthorUrl;
		_loggerService = loggerService;
	}


	/// <summary>
	/// Create a telegraph page from a doujin.
	/// </summary>
	/// <param name="doujin">The doujin for which the telegraph page needs to be created.</param>
	/// <returns>The updated doujin with the new image urls/telegraph link</returns>
	public async Task<Doujin> CreatePageAsync(Doujin doujin)
	{
		string path = await DoujinUtils.Download(doujin);

		var imagesPaths = Directory.GetFiles(path);

		Array.Sort(imagesPaths, (a, b) =>
		{
			int aNumber = int.Parse(Path.GetFileNameWithoutExtension(a));
			int bNumber = int.Parse(Path.GetFileNameWithoutExtension(b));
			return aNumber.CompareTo(bNumber);
		});

		var telegraphFiles = new List<FileToUpload>();

		foreach (string imagesPath in imagesPaths)
		{
			string fileExtension = imagesPath.Split('.').Last();

			telegraphFiles.Add(new FileToUpload
			{
				Bytes = await File.ReadAllBytesAsync(imagesPath),
				Type = "image/" + fileExtension,
			});
		}

		var images = new List<TelegraphFile>();
		try
		{
			for (int i = 0; i < telegraphFiles.Count; i += 10)
			{
				images.AddRange(await _telegraphClient.UploadFiles(telegraphFiles.Skip(i).Take(10).ToList()));
			}
		}
		catch (JsonReaderException e)
		{
			await _loggerService.Log(LogLevel.Error,$"Failed to upload images to telegraph: {doujin.DoujinId}");
		}
		if(images == null || images.Count == 0)
			throw new Exception("Failed to upload images to telegraph");
		
		var content = new List<Node>();

		doujin.ImageUrls.Clear();

		foreach (var image in images)
		{
			content.Add(new Node
				{
					Tag = TagEnum.Img,
					Artibutes = new TagAttributes
					{
						Src = image.Path,
					},
				}
			);
			// Temporary fix until the Telegraph package owner fixes the links
			doujin.ImageUrls.Add(image.Link.Replace("https:/", "https://"));
		}
		doujin.Thumbnail = doujin.ImageUrls.First();
		var page = await _telegraphClient.CreatePage(authorName: _authorName, authorUrl: _authorUrl, content: content,
			title: doujin.Title.Length > 200 ? doujin.Title.Substring(0, 200) : doujin.Title);

		if (page == null)
			throw new Exception("Failed to create telegraph page.");

		doujin.TelegraphUrl = page.Url;

		return doujin;
	}

	/// <summary>
	/// Get the page views for a given telegraph url.
	/// </summary>
	/// <param name="telegraphUrl">The telegraph url</param>
	/// <returns>The number of views if the url is valid</returns>
	/// 
	public async Task<ActionResult<int>> GetPageViews(string telegraphUrl)
	{
		try
		{
			var pageViews = await _telegraphClient.GetViews(telegraphUrl);
			if (pageViews != null)
				return pageViews.Views;

			return new NotFoundResult();
		}
		catch
		{
			return new NotFoundResult();
		}
	}
}