using Kvyk.Telegraph;
using Kvyk.Telegraph.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TelegramBotApi.Models;

namespace TelegramBotApi.Services;

/// <summary>
/// Telegraph service.
/// </summary>
public class TelegraphService
{
	private readonly TelegraphClient _telegraphClient;
	private readonly string _authorName;
	private readonly string _authorUrl;

	/// <summary>
	/// The telegraph service constructor
	/// </summary>
	/// <param name="settings">The telegraph settings.</param>
	public TelegraphService(IOptions<TelegraphSettings> settings)
	{
		_telegraphClient = new TelegraphClient
		{
			AccessToken = settings.Value.AccessToken,
		};
		_authorName = settings.Value.AuthorName;
		_authorUrl = settings.Value.AuthorUrl;
	}


	/// <summary>
	/// Create a telegraph page from a doujin.
	/// </summary>
	/// <param name="doujin">The doujin for which the telegraph page needs to be created.</param>
	/// <returns>The updated doujin with the new image urls/telegraph link</returns>
	public async Task<Doujin> CreatePageAsync(Doujin doujin)
	{
		string path = await Utils.DownloadDoujin.Download(doujin);

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

		var images = await _telegraphClient.UploadFiles(telegraphFiles);

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

			doujin.ImageUrls.Add(image.Link);
		}

		var page = await _telegraphClient.CreatePage(authorName: _authorName, authorUrl: _authorUrl, content: content,
			title: doujin.Title);

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