using System.Globalization;
using System.IO.Compression;
using TelegramBotApi.Models;

namespace TelegramBotApi.Utils;

/// <summary>
/// Utils for doujins.
/// </summary>
public static class  DoujinUtils
{
	/// <summary>
	/// Download a doujin.
	/// </summary>
	/// <param name="d">The doujin for which the images needs to be downloaded.</param>
	/// <returns>The path of the folder containing the images.</returns>
	public static async Task<string> Download(Doujin d)
	{
		string path = Path.Combine("doujins", $"{d.Source.ToString()}/{d.DoujinId}");

		Directory.CreateDirectory(path);

		var tasks = d.ImageUrls.Select((url, i) => DownloadImage(url, path, i)).ToList();

		await Task.WhenAll(tasks);

		return path;
	}


	/// <summary>
	/// Download an image from a url and save it to a path.
	/// </summary>
	/// <param name="url">The image url.</param>
	/// <param name="path">The path where the image should be located at.</param>
	/// <param name="index">The index for the file number.</param>
	/// <returns>The image path.</returns>
	private static async Task<string> DownloadImage(string url, string path, int index)
	{
		using var client = new HttpClient();

		string filePath = Path.Combine(path, $"{index}.{url.Split('.').Last()}");

		byte[] result = await client.GetByteArrayAsync(url);

		await File.WriteAllBytesAsync(filePath, result);

		return filePath;
	}


	/// <summary>
	///  Zip a doujin.
	/// </summary>
	/// <param name="doujin">The doujin to be ziped</param>
	/// <returns>The zip file.</returns>
	public static async Task<string> ZipDoujin(Doujin doujin)
	{
		string downloadFolderPath = await Download(doujin);
		await CreateDescriptorFile(doujin, downloadFolderPath);
		string zipFolderPath = Path.Combine("zips", $"{doujin.Source.ToString()}");
		Directory.CreateDirectory(zipFolderPath);
		string zipFilePath = Path.Combine(zipFolderPath, $"{doujin.DoujinId}.zip");
		ZipFile.CreateFromDirectory(downloadFolderPath, zipFilePath);
		Directory.Delete(downloadFolderPath, true);
		
		return zipFilePath;
	}


	/// <summary>
	///	Create a descriptor file for a doujin.
	/// </summary>
	/// <param name="doujin"> The doujin for which the descriptor file needs to be created.</param>
	/// <param name="filePath"> The path where the descriptor file should be located at.</param>
	private static async Task CreateDescriptorFile(Doujin doujin, string filePath)
	{
		string descriptorText =
			$"Title : {doujin.Title}\nRating : {doujin.Rating}\nCategory : {doujin.Category}\nTags : " +
			$"{string.Join(" ", doujin.Tags)}\nOriginal URL : {doujin.Url}\nTelegraph URL : {doujin.TelegraphUrl}\n" +
			$"Posted : {ConvertUnixTimestampToDateString(doujin.Posted)}";
		
		await File.WriteAllTextAsync($"{filePath}/{doujin.DoujinId}.txt",descriptorText);
	}

	/// <summary>
	///  Convert unix timestamp to date string.
	/// </summary>
	/// <param name="unixTimestamp"> The unix timestamp.</param>
	/// <returns></returns>
	private static string ConvertUnixTimestampToDateString(long unixTimestamp)
	{
		var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);

		return dateTimeOffset.LocalDateTime.ToString(CultureInfo.InvariantCulture);
	}
}