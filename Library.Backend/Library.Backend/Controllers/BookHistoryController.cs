using Library.Backend.Data;
using Library.Backend.DTOs.BookHistory;
using Library.Backend.Queries;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Authorize]
[Route("bookHistory")]
[ApiController]
public class BookHistoryController : ControllerBase
{
    private readonly BookHistoryService _bookHistoryService;

    public BookHistoryController(BookHistoryService bookHistoryService)
    {
        _bookHistoryService = bookHistoryService;
    }

    [HttpGet("getHistory")]
    public async Task<ActionResult<PaginatedList<BookHistoryInfoDto>>> GetBookHistory(
        [FromQuery] BookHistoryQueryParameters parameters)
    {
        return Ok(await _bookHistoryService.GetBooksHistories(parameters));
    }

    [HttpGet("getHistoryById")]
    public async Task<ActionResult<BookHistoryInfoDto>> GetBookHistoryById(int id)
    {
        var bookHistory = await _bookHistoryService.GetHistoryById(id);
        return Ok(bookHistory);
    }

    [HttpGet("getHistoryByBookId")]
    public async Task<ActionResult<BookHistoryInfoDto>> GetBookHistoryByBookId(int bookId)
    {
        var bookHistory = await _bookHistoryService.GetHistoryByBookId(bookId);
        return Ok(bookHistory);
    }

    [HttpGet("getHistoryByUserId")]
    public async Task<ActionResult<BookHistoryInfoDto>> GetBookHistoryByUserId(string userId)
    {
        var bookHistory = await _bookHistoryService.GetHistoryByUserId(userId);
        return Ok(bookHistory);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("clearHistory")]
    public async Task<ActionResult<bool>> ClearHistory(int id)
    {
        await _bookHistoryService.ClearBookHistory(id);
        return Ok("Book history has been cleared");
    }
}