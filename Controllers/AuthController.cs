using BookStoreApi.Models;
using BookStoreApi.Security;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userRepository)
    {
        _userService = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
    {
        var user = await _userService.GetByEmailAsync(model.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            return BadRequest(new { message = "Email or password is incorrect" });
        }

        var token = JwtUtils.GenerateJwtToken(user!);

        return Ok(new
        {
            Id = user?.Id,
            Email = user?.Email,
            Token = token
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _userService.IsEmailAvailableAsync(model.Email))
        {
            return BadRequest(new { message = "Email is already taken" });
        }

        var user = new User
        {
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Roles = new[] { "User" }
        };

        await _userService.InsertAsync(user);

        return Ok( new { message = "User created successfully", data = user });
    }
}