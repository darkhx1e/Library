using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.Book;

public class UpdateBookDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)] 
    public required string Title { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 3)] 
    public required string Author { get; set; }
    [Required]
    public required int[] GenreIds { get; set; } = [];

    public DateTime? PublishDate { get; set; }
}