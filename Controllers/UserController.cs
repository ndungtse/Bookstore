using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    [HttpGet]
    public async Task<List<User>> Get() =>
        await _userService.GetAllAsync();


    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> Post(User newUser)
    {
        if (!await _userService.IsEmailAvailableAsync(newUser.Email))
        {
            return BadRequest(new { message = "Email is already taken" });
        }

        await _userService.InsertAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        if (user.Id != updatedUser.Id)
        {
            return BadRequest();
        }

        if (!await _userService.IsEmailAvailableAsync(updatedUser.Email))
        {
            return BadRequest(new { message = "Email is already taken" });
        }

        await _userService.UpdateAsync(updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _userService.DeleteAsync(user);

        return NoContent();
    }
}