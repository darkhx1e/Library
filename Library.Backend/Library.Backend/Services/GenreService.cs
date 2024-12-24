using Library.Backend.Data;
using Library.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Services;

public class GenreService
{
    private readonly ApplicationDbContext _context;

    public GenreService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genre>> GetAllGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    public async Task<Genre> AddGenre(string genreName)
    {
        if (genreName.Any(char.IsDigit))
        {
            throw new Exception("Genre name cannot contain numbers");
        }
        
        if (_context.Genres.Any(g => g.Name.ToLower() == genreName.ToLower()))
        {
            throw new Exception($"Genre {genreName} already exists");
        }
        
        var genre = new Genre
        {
            Name = genreName
        };
        
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task<Genre> UpdateGenre(Genre newGenre)
    {
        var genre = await _context.Genres.FindAsync(newGenre.Id);

        if (genre == null)
        {
            throw new Exception("Genre not found");
        }
        
        if (newGenre.Name.Any(char.IsDigit))
        {
            throw new Exception("Genre name cannot contain numbers");
        }
        
        if (_context.Genres.Any(g => g.Name.ToLower() == newGenre.Name.ToLower()))
        {
            throw new Exception($"Genre {newGenre.Name} already exists");
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
            throw new Exception("Genre not found");
        }
        
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return true;
    }
}