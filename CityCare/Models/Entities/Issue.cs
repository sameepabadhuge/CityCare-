using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Issue
{
    public int Id { get; set; }  // ✅ Primary Key

    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required, MaxLength(1000)]
    public string Description { get; set; } = "";

    [Required, MaxLength(60)]
    public string Category { get; set; } = "General"; // Road, Water, Waste, etc.

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending";   // Pending/InProgress/Resolved

    [MaxLength(150)]
    public string? LocationText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int UserId { get; set; }
    public User? User { get; set; }

    // Navigation
    public ICollection<IssueImage> Images { get; set; } = new List<IssueImage>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
