using System.Security.Claims;
using Library.Backend.DTOs.Book;
using Library.Backend.Models;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BooksController(BookService bookService, UserManager<ApplicationUser> userManager)
    {
        _bookService = bookService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookInfoDto>>> GetAllBooks()
    {
        return Ok(await _bookService.GetAllBooks());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookInfoDto>> GetBookById(int id)
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

    [HttpPost("multiple")]
    public async Task<ActionResult> AddMultipleBooks(List<CreateBookDto>? books)
    {
        if (books == null || !books.Any())
        {
            return BadRequest("No books provided.");
        }
        
        try
        {
            await _bookService.AddMultipleBooks(books);
            return Ok("Books added successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<BookInfoDto>> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var updatedBook = await _bookService.UpdateBook(id, updateBookDto);

        if (updatedBook == null)
        {
            return NotFound();
        }

        return Ok(updatedBook);
    }

    [HttpPost("{bookId}/take")]
    public async Task<ActionResult<bool>> TakeBook(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }
        
        var user = await _userManager.FindByEmailAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        try
        {
            await _bookService.TakeBook(bookId, user);
            return Ok("Book successfully taken!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}