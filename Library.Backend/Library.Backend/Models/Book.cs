﻿using System.ComponentModel.DataAnnotations;

namespace Library.Backend.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? PublishDate { get; set; }
}