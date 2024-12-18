using Library.Backend.Data;
using Library.Backend.DTOs.Book;
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

    public async Task<List<Book>> GetAllBooks()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetBookById(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<Book> CreateBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
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
    
}