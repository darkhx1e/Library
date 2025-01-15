using System.ComponentModel.DataAnnotations;

namespace Library.Backend.Models;

public class Genre
{
    public int Id { get; set; }

    [StringLength(25, MinimumLength = 3)] public string Name { get; set; }

    public ICollection<BookGenre> BookGenres { get; set; }
}