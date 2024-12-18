using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.Book;

public class CreateBookDto
{
    [Required] 
    [StringLength(50)] 
    [MinLength(3)] 
    public required string Title { get; set; }
    
    [Required] 
    [StringLength(50)] 
    [MinLength(3)]
    public required string Author { get; set; }
    
    public DateTime? PublishedDate { get; set; }
}