using System.ComponentModel.DataAnnotations;

namespace Library.Backend.Models;

public class Book
{
    public int Id { get; set; }
    [StringLength(100)]
    public required string Title { get; set; }
    [StringLength(100)]
    public required string Author { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? PublishDate { get; set; }
    public string? TakenByUserId { get; set; }
    public ApplicationUser? TakenByUser { get; set; }
    public required ICollection<BookGenre> BookGenres { get; set; }
}