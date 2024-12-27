using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Utils;

public class PaginatedList<T>(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
{
    public IReadOnlyList<T> Items { get; set; } = items;
    public int PageNumber { get; set; } = pageNumber;
    public int TotalPages { get; set; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    public int TotalCount { get; set; } = totalCount;

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
        return new PaginatedList<T>(items, pageNumber, pageSize, count);
    }
}