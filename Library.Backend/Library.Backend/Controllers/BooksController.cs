using Library.Backend.DTOs.Book;
using Library.Backend.Models;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
    {
        return Ok(await _bookService.GetAllBooks());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> AddBook(CreateBookDto bookDto)
    {
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            CreatedDate = DateTime.UtcNow,
            PublishDate = bookDto.PublishedDate,
            IsAvailable = true
        };

        await _bookService.CreateBook(book);
        return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<Book>> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var updatedBook = await _bookService.UpdateBook(id, updateBookDto);

        if (updatedBook == null)
        {
            return NotFound();
        }

        return Ok(updatedBook);
    }
}