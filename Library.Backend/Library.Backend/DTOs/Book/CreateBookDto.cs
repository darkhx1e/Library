using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Library.Backend.DTOs.Book;

public class CreateBookDto
{
    [Required] 
    [StringLength(50)] 
    [MinLength(3)]
    public string Title { get; set; }
    [Required] 
    [StringLength(50)] 
    [MinLength(3)]
    public string Author { get; set; }
    public DateTime? PublishedDate { get; set; }
}