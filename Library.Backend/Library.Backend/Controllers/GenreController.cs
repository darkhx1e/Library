using Library.Backend.DTOs.Genre;
using Library.Backend.Models;
using Library.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Backend.Controllers;

[Route("genre")]
[ApiController]
[Authorize]
public class GenreController : ControllerBase
{
    private readonly GenreService _genreService;

    public GenreController(GenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet("getAllGenres")]
    public async Task<ActionResult<List<GenreInfoDto>>> GetAllGenres()
    {
        return Ok(await _genreService.GetAllGenres());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("addGenre")]
    public async Task<ActionResult> AddGenre([FromBody] string name)
    {
        await _genreService.AddGenre(name);
        return Ok("Genre added");
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("updateGenre")]
    public async Task<ActionResult> UpdateGenre([FromBody] UpdateGenreDto genre)
    {
        await _genreService.UpdateGenre(genre);
        return Ok("Genre updated");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("deleteGenre")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        await _genreService.DeleteGenre(id);
        return Ok("Genre deleted");
    }
}