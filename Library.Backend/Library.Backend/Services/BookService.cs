using Library.Backend.Data;
using Library.Backend.DTOs.Book;
using Library.Backend.DTOs.User;
using Library.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Services;

public class BookService
{
    private readonly ApplicationDbContext _context;

    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookInfoDto>> GetAllBooks()
    {
        var books = await _context.Books
            .Include(b => b.TakenByUser)
            .ToListAsync();
        
        return books.Select(book => new BookInfoDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CreatedDate = book.CreatedDate,
            PublishDate = book.PublishDate,
            TakenByUser = book.TakenByUser == null ? null : new UserInfoDto
            {
                Id = book.TakenByUser.Id,
                Email = book.TakenByUser.Email,
                Name = book.TakenByUser.Name,
                Surname = book.TakenByUser.Surname,
            }
        });
    }

    public async Task<BookInfoDto?> GetBookById(int id)
    {
        var book = await _context.Books
            .Include(b => b.TakenByUser)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return null;

        var bookDto = new BookInfoDto()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CreatedDate = book.CreatedDate,
            IsAvailable = book.IsAvailable,
            PublishDate = book.PublishDate,
            TakenByUser = book.TakenByUser == null
                ? null
                : new UserInfoDto
                {
                    Id = book.TakenByUser.Id,
                    Email = book.TakenByUser.Email,
                    Name = book.TakenByUser.Name,
                    Surname = book.TakenByUser.Surname,
                }

        };

        return bookDto;
    }

    public async Task<Book> CreateBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task AddMultipleBooks(List<CreateBookDto> createBookDtos)
    {
        var books = createBookDtos.Select(bookDto => new Book
        {
            Author = bookDto.Author,
            Title = bookDto.Title,
            CreatedDate = DateTime.UtcNow,
            PublishDate = bookDto.PublishedDate,
            IsAvailable = true
        }).ToList();
        
        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();
    }

    public async Task<Book?> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var book = await _context.Books.FindAsync(id);
        
        if (book == null) return null;
        
        if (updateBookDto.Title != null)
        {
            book.Title = updateBookDto.Title;
        }

        if (updateBookDto.Author != null)
        {
            book.Author = updateBookDto.Author;
        }

        if (updateBookDto.PublishDate.HasValue)
        {
            book.PublishDate = updateBookDto.PublishDate;
        }

        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<bool> TakeBook(int bookId, ApplicationUser user)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            throw new ArgumentException("Book not found.");
        }

        if (!book.IsAvailable)
        {
            throw new ArgumentException("Book is already taken.");
        }
        
        book.IsAvailable = false;
        book.TakenByUserId = user.Id;

        await _context.SaveChangesAsync();
        return true;
    }
    
}