using Library.Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Backend.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    public DbSet<BookHistory> BookHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<BookGenre>()
            .HasKey(bg => new { bg.BookId, bg.GenreId });  

        modelBuilder.Entity<BookGenre>()
            .HasOne(bg => bg.Book)
            .WithMany(b => b.BookGenres)
            .HasForeignKey(bg => bg.BookId);

        modelBuilder.Entity<BookGenre>()
            .HasOne(bg => bg.Genre)
            .WithMany(g => g.BookGenres)
            .HasForeignKey(bg => bg.GenreId);

        modelBuilder.Entity<Genre>()
            .Ignore(b => b.BookGenres);

        modelBuilder.Entity<BookHistory>()
            .HasKey(bh => bh.Id);
        
        modelBuilder.Entity<BookHistory>()
            .HasOne(bh => bh.Book)
            .WithMany(b => b.BookHistories)
            .HasForeignKey(bh => bh.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<BookHistory>()
            .HasOne(bh => bh.User)
            .WithMany(u => u.BookHistories)
            .HasForeignKey(bh => bh.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}