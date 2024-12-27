using Library.Backend.Data;
using Library.Backend.DTOs.User;
using Library.Backend.Models;
using Library.Backend.Utils;
using Microsoft.AspNetCore.Identity;

namespace Library.Backend.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<UserInfoDto> GetUserInfo(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            throw new CustomException($"User not found", StatusCodes.Status404NotFound);
        }

        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
        };
    }

    public async Task<bool> UpdateUserInfo(string userId, UpdateUserInfoDto updateUserInfoDto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new CustomException($"User not found", StatusCodes.Status404NotFound);
        }
        
        user.Name = updateUserInfoDto.Name;
        user.Surname = updateUserInfoDto.Surname;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeUserPassword(string userId, ChangeUserPasswordDto changeUserPasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new CustomException($"User not found.", StatusCodes.Status404NotFound);
        }
        
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var verificationResult =
            passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, changeUserPasswordDto.CurrentPassword);

        if (verificationResult != PasswordVerificationResult.Success)
        {
            throw new CustomException("Current password is incorrect.", StatusCodes.Status400BadRequest);
        }
        
        user.PasswordHash = passwordHasher.HashPassword(user, changeUserPasswordDto.NewPassword);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}