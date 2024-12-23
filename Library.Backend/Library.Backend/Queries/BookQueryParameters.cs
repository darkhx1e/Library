namespace Library.Backend.Queries;

public class BookQueryParameters
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}