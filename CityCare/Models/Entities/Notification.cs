using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = "";   // who receives it
    public User? User { get; set; }

    public int? IssueId { get; set; }
    public Issue? Issue { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required, MaxLength(500)]
    public string Message { get; set; } = "";

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
