using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Notification
{
    public int Id { get; set; } // ✅ Primary Key

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [Required, MaxLength(500)]
    public string Message { get; set; } = "";

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional link to an issue
    public int? IssueId { get; set; }
    public Issue? Issue { get; set; }
}
