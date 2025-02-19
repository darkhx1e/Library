﻿using Microsoft.AspNetCore.Identity;

namespace Library.Backend.Models;

public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public ICollection<BookHistory> BookHistories { get; set; } = new List<BookHistory>();
}