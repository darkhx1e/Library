using Library.Backend.Data;
using Library.Backend.DTOs.Book;
using Library.Backend.DTOs.Genre;
using Library.Backend.DTOs.User;
using Library.Backend.Models;
using Library.Backend.Queries;
using Library.Backend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Services;

public class BookService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<PaginatedList<BookInfoDto>> GetAllBooks(BookQueryParameters bookQueryParameters)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrEmpty(bookQueryParameters.Title))
        {
            query = query.Where(b => b.Title.ToLower().Contains(bookQueryParameters.Title.ToLower()));
        }

        if (!string.IsNullOrEmpty(bookQueryParameters.Author))
        {
            query = query.Where(b => b.Author.ToLower().Contains(bookQueryParameters.Author.ToLower()));
        }

        if (bookQueryParameters.GenreIds != null && bookQueryParameters.GenreIds.Any())
        {
            query = query.Where(b => b.BookGenres.Any(bg => bookQueryParameters.GenreIds.Contains(bg.GenreId)));
        }

        var source = query
            .Include(b => b.TakenByUser)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Select(book => new BookInfoDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CreatedDate = book.CreatedDate,
                PublishDate = book.PublishDate,
                IsAvailable = book.IsAvailable,
                CoverImagePath = book.CoverImagePath,
                TakenByUser = book.TakenByUser == null
                    ? null
                    : new UserInfoDto
                    {
                        Id = book.TakenByUser.Id,
                        Email = book.TakenByUser.Email,
                        Name = book.TakenByUser.Name,
                        Surname = book.TakenByUser.Surname,
                    },
                Genres = book.BookGenres.Select(bg => new GenreInfoDto
                    {
                        Id = bg.GenreId,
                        Name = bg.Genre.Name,
                    }
                ).ToList(),
            });

        return await PaginatedList<BookInfoDto>.CreateAsync(source, bookQueryParameters.Page,
            bookQueryParameters.PageSize);
    }

    public async Task<BookInfoDto?> GetBookById(int id)
    {
        var book = await _context.Books
            .Include(b => b.TakenByUser)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            throw new CustomException("Book not found", StatusCodes.Status404NotFound);
        }

        var bookDto = new BookInfoDto()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            CreatedDate = book.CreatedDate,
            IsAvailable = book.IsAvailable,
            PublishDate = book.PublishDate,
            CoverImagePath = book.CoverImagePath,
            TakenByUser = book.TakenByUser == null
                ? null
                : new UserInfoDto
                {
                    Id = book.TakenByUser.Id,
                    Email = book.TakenByUser.Email,
                    Name = book.TakenByUser.Name,
                    Surname = book.TakenByUser.Surname,
                },
            Genres = book.BookGenres.Select(bg => new GenreInfoDto
            {
                Id = bg.GenreId,
                Name = bg.Genre.Name,
            }).ToList()
        };

        return bookDto;
    }

    public async Task<Book> CreateBook(CreateBookDto bookDto)
    {
        await CoverImageUtils.ValidateCoverImage(bookDto.CoverImage);
        
        var fileHash = await CoverImageUtils.ComputeCoverImageHash(bookDto.CoverImage);
        
        var existingBookWithSameCover = await _context.Books
            .FirstOrDefaultAsync(b => b.CoverImageHash == fileHash);
        
        string coverImagePath;

        if (existingBookWithSameCover != null)
        {
            coverImagePath = existingBookWithSameCover.CoverImagePath;
        }
        else
        {
            var uploadsFolder = Path.Combine("wwwroot", "images", "covers");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDto.CoverImage.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
        
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bookDto.CoverImage.CopyToAsync(stream);
            }

            coverImagePath = Path.Combine("images", "covers", fileName);
        }
        
        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            CreatedDate = DateTime.UtcNow,
            PublishDate = bookDto.PublishedDate,
            IsAvailable = true,
            BookGenres = new List<BookGenre>(),
            CoverImagePath = coverImagePath,
            CoverImageHash = fileHash,
        };
        
        foreach (var genreId in bookDto.GenreIds)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre != null)
            {
                book.BookGenres.Add(new BookGenre
                {
                    Book = book,
                    GenreId = genreId
                });
            }
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    /*public async Task AddMultipleBooks(List<CreateBookDto> createBookDtos)
    {
        var books = createBookDtos.Select(bookDto => new Book
        {
            Author = bookDto.Author,
            Title = bookDto.Title,
            CreatedDate = DateTime.UtcNow,
            PublishDate = bookDto.PublishedDate,
            IsAvailable = true
        }).ToList();

        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();
    }*/

    public async Task<Book?> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var book = await _context.Books
            .Include(b => b.BookGenres)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            throw new CustomException("Book not found", StatusCodes.Status404NotFound);
        }

        book.Title = updateBookDto.Title;
        book.Author = updateBookDto.Author;
        book.PublishDate = updateBookDto.PublishDate;

        _context.BookGenres.RemoveRange(book.BookGenres);

        foreach (var genreId in updateBookDto.GenreIds)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre != null)
            {
                book.BookGenres.Add(new BookGenre
                {
                    Book = book,
                    GenreId = genreId
                });
            }
        }

        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<bool> DeleteBook(int bookId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            throw new CustomException("Book not found.", StatusCodes.Status404NotFound);
        }

        if (!book.IsAvailable)
        {
            throw new CustomException("Can't delete this book because it is already taken.",
                StatusCodes.Status400BadRequest);
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TakeBook(int bookId, string userId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            throw new CustomException("Book not found.", StatusCodes.Status404NotFound);
        }

        if (!book.IsAvailable)
        {
            throw new CustomException("Book is already taken.", StatusCodes.Status400BadRequest);
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new CustomException("User not found.", StatusCodes.Status404NotFound);
        }

        book.IsAvailable = false;
        book.TakenByUserId = user.Id;

        _context.BookHistories.Add(new BookHistory
        {
            Book = book,
            BookId = bookId,
            User = user,
            UserId = user.Id,
            BorrowDate = DateTime.UtcNow,
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReturnBook(int bookId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            throw new CustomException("Book not found.", StatusCodes.Status404NotFound);
        }

        if (book.IsAvailable)
        {
            throw new CustomException("Book is not taken.", StatusCodes.Status400BadRequest);
        }

        var bookHistory =
            await _context.BookHistories.FirstOrDefaultAsync(h => h.BookId == bookId && h.UserId == book.TakenByUserId);

        if (bookHistory != null)
        {
            bookHistory.ReturnDate = DateTime.UtcNow;
        }

        book.IsAvailable = true;
        book.TakenByUserId = null;
        await _context.SaveChangesAsync();
        return true;
    }
}