using Library.Backend.DTOs.Genre;
using Library.Backend.DTOs.User;

namespace Library.Backend.DTOs.Book;

public class BookInfoDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? PublishDate { get; set; }
    public UserInfoDto? TakenByUser { get; set; }
    public required List<GenreInfoDto> Genres { get; set; }

    public string? CoverImagePath { get; set; }
}