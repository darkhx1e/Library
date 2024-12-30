using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.Book;

public class CreateBookDto
{
    [Required] 
    [StringLength(100, MinimumLength = 3)] 
    public required string Title { get; set; }
    [Required] 
    [StringLength(100, MinimumLength = 3)] 
    public required string Author { get; set; }
    [Required]
    public required int[] GenreIds { get; set; }
    public DateTime? PublishedDate { get; set; }
    [Required]
    public required IFormFile CoverImage { get; set; }
}