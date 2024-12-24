using Library.Backend.DTOs.Book;
using Library.Backend.DTOs.User;

namespace Library.Backend.DTOs.BookHistory;

public class BookHistoryInfoDto
{
    public int Id { get; set; }
    public UserInfoDto User { get; set; }
    public BookInfoDto Book { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}