using Library.Backend.Data;
using Library.Backend.DTOs.Book;
using Library.Backend.DTOs.BookHistory;
using Library.Backend.DTOs.Genre;
using Library.Backend.DTOs.User;
using Library.Backend.Models;
using Library.Backend.Queries;
using Library.Backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Services;

public class BookHistoryService
{
    private readonly ApplicationDbContext _context;

    public BookHistoryService(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    public async Task<PaginatedList<BookHistoryInfoDto>> GetBooksHistories(BookHistoryQueryParameters parameters)
    {
        var query = _context.BookHistories
            .Include(bh => bh.Book)
            .ThenInclude(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(bh => bh.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Title))
            query = query.Where(bh => bh.Book.Title.ToLower().Contains(parameters.Title.ToLower()));

        if (!string.IsNullOrWhiteSpace(parameters.Author))
            query = query.Where(bh => bh.Book.Title.ToLower().Contains(parameters.Author.ToLower()));

        if (!string.IsNullOrWhiteSpace(parameters.UserName))
            query = query.Where(bh => bh.User.Name.ToLower().Contains(parameters.UserName.ToLower()));

        if (parameters.StartDate.HasValue) query = query.Where(bh => bh.BorrowDate >= parameters.StartDate);

        if (parameters.EndDate.HasValue) query = query.Where(bh => bh.BorrowDate <= parameters.EndDate);

        var source = query.Select(bh => MapToDto(bh));

        return await PaginatedList<BookHistoryInfoDto>.CreateAsync(source, parameters.Page, parameters.PageSize);
    }

    public async Task<BookHistoryInfoDto> GetHistoryById(int id)
    {
        var bookHistory = await _context.BookHistories
            .Include(bh => bh.User)
            .Include(bh => bh.Book)
            .ThenInclude(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(bh => bh.Id == id);

        if (bookHistory == null)
            throw new CustomException($"Book history with id: {id} not found", StatusCodes.Status404NotFound);

        return MapToDto(bookHistory);
    }

    public async Task<BookHistoryInfoDto> GetHistoryByBookId(int bookId)
    {
        var bookHistory = await _context.BookHistories
            .Include(bh => bh.User)
            .Include(bh => bh.Book)
            .ThenInclude(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(bh => bh.BookId == bookId);

        if (bookHistory == null)
            throw new CustomException($"Book history with bookId: {bookId} not found", StatusCodes.Status404NotFound);

        return MapToDto(bookHistory);
    }

    public async Task<BookHistoryInfoDto> GetHistoryByUserId(string userId)
    {
        var bookHistory = await _context.BookHistories
            .Include(bh => bh.User)
            .Include(bh => bh.Book)
            .ThenInclude(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(bh => bh.UserId == userId);

        if (bookHistory == null)
            throw new CustomException($"Book history with userId: {userId} not found", StatusCodes.Status404NotFound);

        return MapToDto(bookHistory);
    }

    public async Task<bool> ClearBookHistory(int id)
    {
        var bookHistory = await _context.BookHistories
            .Include(bh => bh.Book)
            .FirstOrDefaultAsync(bh => bh.Id == id);

        if (bookHistory == null)
            throw new CustomException($"Book history with id: {id} not found", StatusCodes.Status404NotFound);

        if (!bookHistory.Book.IsAvailable)
            throw new CustomException("History of a taken book can't be deleted", StatusCodes.Status400BadRequest);

        _context.BookHistories.Remove(bookHistory);
        await _context.SaveChangesAsync();
        return true;
    }

    private static BookHistoryInfoDto MapToDto(BookHistory bookHistory)
    {
        return new BookHistoryInfoDto
        {
            Id = bookHistory.Id,
            Book = new BookInfoDto
            {
                Id = bookHistory.BookId,
                Title = bookHistory.Book.Title,
                Author = bookHistory.Book.Author,
                IsAvailable = bookHistory.Book.IsAvailable,
                CreatedDate = bookHistory.Book.CreatedDate,
                Genres = bookHistory.Book.BookGenres
                    .Select(bg => new GenreInfoDto { Id = bg.GenreId, Name = bg.Genre.Name })
                    .ToList()
            },
            User = new UserInfoDto
            {
                Id = bookHistory.UserId,
                Email = bookHistory.User.Email,
                Name = bookHistory.User.Name,
                Surname = bookHistory.User.Surname
            },
            BorrowDate = bookHistory.BorrowDate,
            ReturnDate = bookHistory.ReturnDate
        };
    }
}