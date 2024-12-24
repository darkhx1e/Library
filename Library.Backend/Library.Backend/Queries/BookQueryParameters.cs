namespace Library.Backend.Queries;

public class BookQueryParameters
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<int>? GenreIds { get; set; }
}