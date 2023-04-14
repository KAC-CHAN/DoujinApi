using Microsoft.AspNetCore.Mvc;
using TelegramBotApi.Models;
using TelegramBotApi.Services;

namespace TelegramBotApi.Controllers;

/// <summary>
/// The controller for the users collection.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController
{
	private readonly UserService _userService;
	/// <summary>
	/// The constructor for the users controller.
	/// </summary>
	/// <param name="userService"></param>
	public UsersController(UserService userService)
	{
		_userService = userService;
	}
	
	/// <summary>
	/// Get the count of all the users in the database.
	/// </summary>
	/// <returns>The count of all users.</returns>
	[HttpGet("count")]
	public async Task<int> Count() => await _userService.GetCountAsync();
	
	/// <summary>
	/// Get all the users from the database.
	/// </summary>
	/// <returns>All the users.</returns>
	[HttpGet]
	[Produces("application/json")]
	public async Task<List<User>> Get() => await _userService.GetAsync();

	/// <summary>
	/// Get a user by it's document id.
	/// </summary>
	/// <param name="id">The document id.</param>
	/// <returns>A user</returns>
	[HttpGet("{id:length(24)}")]
	public async Task<ActionResult<User>> Get(string id)
	{
		var user = await _userService.GetAsyncDocId(id);
		if(user == null)
			return new NotFoundResult();
		return user;
		
	}
	/// <summary>
	/// Get a user by it's user id.
	/// </summary>
	/// <param name="id">The telegram ID of the user.</param>
	/// <returns>A user</returns>
	[HttpGet("userId/{id:long}")]
	public async Task<ActionResult<User>> GetId(long id)
	{
		Console.Write(id);
		var user = await _userService.GetAsyncId(id);
		if(user == null)
			return new NotFoundResult();
		return user;
	}
	
	/// <summary>
	/// Create a new user in the database.
	/// </summary>
	/// <param name="user">The new user.</param>
	/// <returns></returns>
	[HttpPost]
	public async Task<IActionResult> Create(User user)
	{
		await _userService.CreateAsync(user);

		return new CreatedResult($"/api/v1/users/{user.Id}", user);
	}
	
	/// <summary>
	/// Update a user in the database.
	/// </summary>
	/// <param name="id">The document id of the user.</param>
	/// <param name="updatedUser">The updated user.</param>
	/// <returns></returns>
	[HttpPut("{id:length(24)}")]
	public async Task<IActionResult> Update(string id, User updatedUser)
	{
		var user = await _userService.GetAsyncDocId(id);
		if(user == null)
			return new NotFoundResult();
		await _userService.UpdateAsync(updatedUser);
		return new OkResult();
	}
	
	/// <summary>
	/// Delete a user from the database.
	/// </summary>
	/// <param name="id">The user's document id.</param>
	/// <returns>204 on success.</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id)
	{
		var user = await _userService.GetAsyncDocId(id);
		if(user == null)
			return new NoContentResult();
		await _userService.DeleteAsync(id);
		return new NoContentResult();
	}
}