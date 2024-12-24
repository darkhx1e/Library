using System.Security.Claims;
using Library.Backend.Data;
using Library.Backend.DTOs.Book;
using Library.Backend.Models;
using Library.Backend.Queries;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Route("books")]
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

    [HttpGet("getAllBooks")]
    public async Task<ActionResult<PaginatedList<BookInfoDto>>> GetAllBooks([FromQuery] BookQueryParameters bookQueryParameters)
    {
        return Ok(await _bookService.GetAllBooks(bookQueryParameters));
    }

    [HttpGet("getBookById")]
    public async Task<ActionResult<BookInfoDto>> GetBookById(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null) return NotFound();
        
        return Ok(book);
    }

    [HttpPost("addBook")]
    public async Task<IActionResult> AddBook([FromBody] CreateBookDto bookDto)
    {
        try
        {
            await _bookService.CreateBook(bookDto);
            return Ok("Book created");
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    /*[HttpPost("addMultipleBooks")]
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
    }*/

    [HttpPatch("updateBook/{id}")]
    public async Task<ActionResult<BookInfoDto>> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var updatedBook = await _bookService.UpdateBook(id, updateBookDto);

        if (updatedBook == null)
        {
            return NotFound();
        }

        return Ok(updatedBook);
    }
    
    [HttpDelete("deleteBook")]
    public async Task<ActionResult<bool>> DeleteBook(int id)
    {
        try
        {
            await _bookService.DeleteBook(id);
            return Ok("Book successfully deleted!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("takeBook")]
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
    
    [HttpPost("returnBook")]
    public async Task<ActionResult<bool>> ReturnBook(int bookId)
    {
        try
        {
            await _bookService.ReturnBook(bookId);
            return Ok("Book successfully returned!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}