using System.Security.Claims;
using Library.Backend.DTOs.User;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Route("user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getUserInfo")]
    public async Task<ActionResult<UserInfoDto>> GetUserInfo()
    {
        var userId = User.FindFirstValue("Id");

        if (userId == null)
        {
            return Unauthorized();
        }
        
        var user = await _userService.GetUserInfo(userId);
        return Ok(user);
    }

    [HttpPatch("changeUserInfo")]
    public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoDto updateUserInfoDto)
    {
        var userId = User.FindFirstValue("Id");

        if (userId == null)
        {
            return Unauthorized();
        }
        
        await _userService.UpdateUserInfo(userId, updateUserInfoDto);
        return Ok("User updated");
    }

    [HttpPatch("changePassword")]
    public async Task<IActionResult> ChangePassword(ChangeUserPasswordDto changeUserPasswordDto)
    {
        var userId = User.FindFirstValue("Id");

        if (userId == null)
        {
            return Unauthorized();
        }
        
        await _userService.ChangeUserPassword(userId, changeUserPasswordDto);
        return Ok("Password changed");
    }
    
}