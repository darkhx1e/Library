using Library.Backend.DTOs.User;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        var result = await _authService.RegisterUser(registerUserDto);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            var token = await _authService.LoginUser(email, password);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addRole")]
    public async Task<IActionResult> AddRoleToUser(string userId, string role)
    {
        try
        {
            var result = await _authService.AddRole(userId, role);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Role added successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("removeRole")]
    public async Task<IActionResult> RemoveRoleFromUser(string userId, string role)
    {
        try
        {
            var result = await _authService.RemoveRole(userId, role);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Role added successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}