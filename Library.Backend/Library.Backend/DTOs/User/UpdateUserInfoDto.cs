using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.User;

public class UpdateUserInfoDto
{
    [Required]
    [StringLength(12, MinimumLength = 3)]
    public required string Name { get; set; }
    
    [Required]
    [StringLength(12, MinimumLength = 3)]
    public required string Surname { get; set; }
}