using System.Security.Claims;
using Library.Backend.DTOs.Book;
using Library.Backend.Queries;
using Library.Backend.Services;
using Library.Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Route("books")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly BookService _bookService;

    public BooksController(BookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("getAllBooks")]
    public async Task<ActionResult<PaginatedList<BookInfoDto>>> GetAllBooks(
        [FromQuery] BookQueryParameters bookQueryParameters)
    {
        return Ok(await _bookService.GetAllBooks(bookQueryParameters));
    }

    [HttpGet("getBookById")]
    public async Task<ActionResult<BookInfoDto>> GetBookById(int id)
    {
        var book = await _bookService.GetBookById(id);
        return Ok(book);
    }

    [HttpPost("addBook")]
    public async Task<IActionResult> AddBook([FromForm] CreateBookDto bookDto)
    {
        await _bookService.CreateBook(bookDto);
        return Ok("Book created");
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
        return Ok(updatedBook);
    }

    [HttpDelete("deleteBook")]
    public async Task<ActionResult<bool>> DeleteBook(int id)
    {
        await _bookService.DeleteBook(id);
        return Ok("Book successfully deleted!");
    }

    [HttpPost("takeBook")]
    public async Task<ActionResult<bool>> TakeBook(int bookId)
    {
        var userId = User.FindFirstValue("Id");

        if (userId == null)
        {
            return Unauthorized();
        }
        
        await _bookService.TakeBook(bookId, userId);
        return Ok("Book successfully taken!");
    }

    [HttpPost("returnBook")]
    public async Task<ActionResult<bool>> ReturnBook(int bookId)
    {
        await _bookService.ReturnBook(bookId);
        return Ok("Book successfully returned!");
    }
}