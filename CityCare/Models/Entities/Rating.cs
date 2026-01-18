using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Rating
{
    public int Id { get; set; } // ✅ Primary Key

    [Range(1, 5)]
    public int Stars { get; set; } = 5;

    [MaxLength(400)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK (who rated)
    public int UserId { get; set; }
    public User? User { get; set; }

    // Optional link to issue
    public int? IssueId { get; set; }
    public Issue? Issue { get; set; }
}
