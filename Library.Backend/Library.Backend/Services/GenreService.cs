using Library.Backend.Data;
using Library.Backend.DTOs.Genre;
using Library.Backend.Models;
using Library.Backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Services;

public class GenreService
{
    private readonly ApplicationDbContext _context;

    public GenreService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenreInfoDto>> GetAllGenres()
    {
        return await _context.Genres.Select(g => new GenreInfoDto
        {
            Id = g.Id,
            Name = g.Name
        }).ToListAsync();
    }

    public async Task<Genre> AddGenre(string genreName)
    {
        if (genreName.Any(char.IsDigit))
        {
            throw new CustomException("Genre name cannot contain numbers", StatusCodes.Status400BadRequest);
        }
        
        if (_context.Genres.Any(g => g.Name.ToLower() == genreName.ToLower()))
        {
            throw new CustomException($"Genre {genreName} already exists", StatusCodes.Status400BadRequest);
        }
        
        var genre = new Genre
        {
            Name = genreName
        };
        
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task<Genre> UpdateGenre(UpdateGenreDto newGenre)
    {
        var genre = await _context.Genres.FindAsync(newGenre.Id);

        if (genre == null)
        {
            throw new CustomException("Genre not found", StatusCodes.Status404NotFound);
        }
        
        if (newGenre.Name.Any(char.IsDigit))
        {
            throw new CustomException("Genre name cannot contain numbers", StatusCodes.Status400BadRequest);
        }
        
        if (_context.Genres.Any(g => g.Name.ToLower() == newGenre.Name.ToLower()))
        {
            throw new CustomException($"Genre {newGenre.Name} already exists", StatusCodes.Status400BadRequest);
        }
        
        genre.Name = newGenre.Name;
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task<bool> DeleteGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre == null)
        {
            throw new CustomException("Genre not found", StatusCodes.Status404NotFound);
        }
        
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return true;
    }
}