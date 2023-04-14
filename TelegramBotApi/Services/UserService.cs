using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TelegramBotApi.Models;

namespace TelegramBotApi.Services;

/// <summary>
///  Service for the users.
/// </summary>
public class UserService
{
	private readonly IMongoCollection<User> _users;

	/// <summary>
	///  Constructor for the user service.
	/// </summary>
	/// <param name="settings"> Instance of database settings </param>
	public UserService(IOptions<TgBotDatabaseSettings> settings)
	{
		var client = new MongoClient(settings.Value.ConnectionString);
		var database = client.GetDatabase(settings.Value.DatabaseName);
		_users = database.GetCollection<User>(settings.Value.UsersCollectionName);
	}

	/// <summary>
	/// Get all the users inside the database.
	/// </summary>
	/// <returns>All the users in the database</returns>
	public async Task<List<User>> GetAsync()
	{
		return await _users.Find(user => true).ToListAsync();
	}

	/// <summary>
	/// Get the count of all the users inside the database.
	/// </summary>
	/// <returns>The count of all users in the database.</returns>
	public async Task<int> GetCountAsync()
	{
		return (int) await _users.CountDocumentsAsync(user => true);
	}

	/// <summary>
	/// Get a user by its user id.
	/// </summary>
	/// <param name="userId">The telegram user id.</param>
	/// <returns>The user</returns>
	public async Task<User> GetAsyncId(Int64 userId)
	{
		return await _users.Find(user => user.UserId == userId).FirstOrDefaultAsync();
	}

	/// <summary>
	/// Get a user by its document id.
	/// </summary>
	/// <param name="docId">The user's document id.</param>
	/// <returns></returns>
	public async Task<User> GetAsyncDocId(string docId)
	{
		return await _users.Find(user => user.Id == docId).FirstOrDefaultAsync();
	}

	/// <summary>
	/// Create a user inside the database.
	/// </summary>
	/// <param name="user">The new user to insert in the database.</param>
	public async Task CreateAsync(User user)
	{
		await _users.InsertOneAsync(user);
	}

	/// <summary>
	/// Update a user inside the database.
	/// </summary>
	/// <param name="user">The user to update</param>
	public async Task UpdateAsync(User user)
	{
		await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
	}

	/// <summary>
	/// Delete a user by it's document id.
	/// </summary>
	/// <param name="docId">The document id of the user.</param>
	public async Task DeleteAsync(string docId)
	{
		await _users.DeleteOneAsync(u => u.Id == docId);
	}
}