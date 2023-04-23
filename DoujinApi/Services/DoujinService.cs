using DoujinApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DoujinApi.Services;

/// <summary>
///  Service for the doujins.
/// </summary>
public class DoujinService
{
	private readonly IMongoCollection<Doujin> _doujinsCollection;

	/// <summary>
	/// Constructor for the doujin service.
	/// </summary>
	/// <param name="settings"> Instance of database settings. </param>
	public DoujinService(IOptions<DatabaseSettings> settings)
	{
		var client = new MongoClient(settings.Value.ConnectionString);
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_doujinsCollection = database.GetCollection<Doujin>(settings.Value.DoujinsCollectionName);
	}

	/// <summary>
	/// Get all the doujins inside the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A list of all the doujins inside the collection.</returns>
	public async Task<List<Doujin>> GetAsync(CancellationToken ct)
	{
		return await _doujinsCollection.Find(_ => true).ToListAsync(cancellationToken: ct);
	}

	/// <summary>
	/// Get a doujin by its doujin id.
	/// </summary>
	/// <param name="doujinId">The doujin's id</param>
	///<param name="ct">Cancellation token</param>
	/// <returns>The doujin if it exists , null if it doesn't</returns>
	public async Task<Doujin> GetAsyncId(string doujinId, CancellationToken ct)
	{
		return await _doujinsCollection.Find(doujin => doujin.DoujinId == doujinId).FirstOrDefaultAsync(cancellationToken:ct);
	}
	
	/// <summary>
	/// Get a doujin by its document id.
	/// </summary>
	/// <param name="docId">The doujin's document _id</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The doujin if it exists , null if it doesn't</returns>
	public async Task<Doujin> GetAsyncDocId(string docId, CancellationToken ct)
	{
		return await _doujinsCollection.Find(doujin => doujin.Id == docId).FirstOrDefaultAsync(cancellationToken:ct);
	}


	/// <summary>
	/// Create a doujin inside the database.
	/// </summary>
	/// <param name="doujin">The doujin to insert in the database</param>
	/// <param name="ct">Cancellation token</param>
	public async Task CreateAsync(Doujin doujin , CancellationToken ct)
	{
		await _doujinsCollection.InsertOneAsync(doujin, cancellationToken: ct);
	}
	
	/// <summary>
	/// Update a doujin inside the database.
	/// </summary>
	/// <param name="updatedDoujin">The doujin to be updated.</param>
	/// <param name="ct">Cancellation token</param>
	public async Task UpdateAsync(Doujin updatedDoujin, CancellationToken ct)
	{
		await _doujinsCollection.ReplaceOneAsync(doujin => doujin.Id == updatedDoujin.Id, updatedDoujin,cancellationToken:ct);
	}
	/// <summary>
	/// Delete a doujin from the database.
	/// </summary>
	/// <param name="docId">The doujin's document id (_id)</param>
	/// <param name="ct">Cancellation token</param>
	public async Task DeleteAsync(string docId, CancellationToken ct)
	{
		await _doujinsCollection.DeleteOneAsync(doujin => doujin.Id == docId, cancellationToken:ct);
	}
	/// <summary>
	/// Get the number of doujins inside the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>The number of doujins.</returns>
	public async Task<int> GetDoujinsCountAsync( CancellationToken ct)
	{
		return (int) await _doujinsCollection.CountDocumentsAsync(_ => true, cancellationToken: ct);
	}
	
	
}