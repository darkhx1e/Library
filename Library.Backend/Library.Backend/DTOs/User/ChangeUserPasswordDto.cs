using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.User;

public class ChangeUserPasswordDto
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string NewPassword { get; set; }
}