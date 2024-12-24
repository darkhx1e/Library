namespace Library.Backend.Queries;

public class BookHistoryQueryParameters
{
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? UserName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}