using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Notification
{
    public int Id { get; set; }

    // who receives notification
    [Required]
    public string UserId { get; set; } = "";
    public User? User { get; set; }

    // related issue
    public int? IssueId { get; set; }
    public Issue? Issue { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [Required, MaxLength(1000)]
    public string Message { get; set; } = "";

    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
