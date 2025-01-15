using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.Backend.DTOs.User;
using Library.Backend.Models;
using Library.Backend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Library.Backend.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            Name = registerUserDto.Name,
            Surname = registerUserDto.Surname,
            UserName = registerUserDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);

        if (result.Succeeded) await _userManager.AddToRoleAsync(user, "User");

        return result;
    }

    public async Task<string?> LoginUser(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new CustomException("User not found.", StatusCodes.Status404NotFound);

        if (!await _userManager.CheckPasswordAsync(user, password))
            throw new CustomException("Invalid password.", StatusCodes.Status400BadRequest);

        var token = await GenerateJwtToken(user);
        return token;
    }

    public async Task<IdentityResult> AddRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) throw new CustomException("User not found.", 404);

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists) throw new CustomException("Role not found.", StatusCodes.Status404NotFound);

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result;
    }

    public async Task<IdentityResult> RemoveRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new CustomException("User not found.", StatusCodes.Status404NotFound);
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists) throw new CustomException("Role not found.", StatusCodes.Status404NotFound);

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result;
    }


    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Id", user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}