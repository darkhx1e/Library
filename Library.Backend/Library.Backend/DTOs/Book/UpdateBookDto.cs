using System.ComponentModel.DataAnnotations;

namespace Library.Backend.DTOs.Book;

public class UpdateBookDto
{
    [StringLength(50)] 
    [MinLength(3)]
    public string? Title { get; set; }
    
    [StringLength(50)] 
    [MinLength(3)]
    public string? Author { get; set; }
    
    public DateTime? PublishDate { get; set; }
}