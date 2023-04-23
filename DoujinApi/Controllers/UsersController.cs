using DoujinApi.Models;
using DoujinApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoujinApi.Controllers;

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
	/// <param name="ct">Cancellation token</param>
	/// <returns>The count of all users.</returns>
	[HttpGet("count")]
	public async Task<int> Count(CancellationToken ct) => await _userService.GetCountAsync(ct);
	
	/// <summary>
	/// Get all the users from the database.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <returns>All the users.</returns>
	[HttpGet]
	[Produces("application/json")]
	public async Task<List<User>> Get(CancellationToken ct) => await _userService.GetAsync(ct);

	/// <summary>
	/// Get a user by it's document id.
	/// </summary>
	/// <param name="id">The document id.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A user</returns>
	[HttpGet("{id:length(24)}")]
	public async Task<ActionResult<User>> Get(string id, CancellationToken ct)
	{
		var user = await _userService.GetAsyncDocId(id, ct);
		if(user == null)
			return new NotFoundResult();
		return user;
		
	}
	/// <summary>
	/// Get a user by it's user id.
	/// </summary>
	/// <param name="id">The telegram ID of the user.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>A user</returns>
	[HttpGet("userId/{id:long}")]
	public async Task<ActionResult<User>> GetId(long id, CancellationToken ct)
	{
		var user = await _userService.GetAsyncId(id, ct);
		if(user == null)
			return new NotFoundResult();
		return user;
	}
	
	/// <summary>
	/// Create a new user in the database.
	/// </summary>
	/// <param name="user">The new user.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns></returns>
	[HttpPost]
	public async Task<IActionResult> Create(User user, CancellationToken ct)
	{
		var existingUser = await _userService.GetAsyncId(user.UserId, ct);
		if(existingUser != null)
			return new ConflictResult();
		
		await _userService.CreateAsync(user, ct);

		return new CreatedResult($"/api/v1/users/{user.Id}", user);
	}
	
	/// <summary>
	/// Update a user in the database.
	/// </summary>
	/// <param name="id">The document id of the user.</param>
	/// <param name="updatedUser">The updated user.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns></returns>
	[HttpPut("{id:length(24)}")]
	public async Task<IActionResult> Update(string id, User updatedUser, CancellationToken ct)
	{
		var user = await _userService.GetAsyncDocId(id, ct);
		if(user == null)
			return new NotFoundResult();
		await _userService.UpdateAsync(updatedUser, ct);
		return new OkResult();
	}
	
	/// <summary>
	/// Delete a user from the database.
	/// </summary>
	/// <param name="id">The user's document id.</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns>204 on success.</returns>
	[HttpDelete("{id:length(24)}")]
	public async Task<IActionResult> Delete(string id, CancellationToken ct)
	{
		var user = await _userService.GetAsyncDocId(id, ct);
		if(user == null)
			return new NoContentResult();
		await _userService.DeleteAsync(id, ct);
		return new NoContentResult();
	}
}