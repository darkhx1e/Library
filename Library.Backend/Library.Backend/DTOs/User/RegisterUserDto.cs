using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.User;

public class RegisterUserDto
{
    [Required]
    [StringLength(12, MinimumLength = 3)]
    public required string Name { get; set; }

    [Required]
    [StringLength(12, MinimumLength = 3)]
    public required string Surname { get; set; }

    [Required] [EmailAddress] public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }
}