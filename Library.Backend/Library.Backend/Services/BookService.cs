using Library.Backend.Data;
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
}