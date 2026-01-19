using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Rating
{
    public int Id { get; set; }

    public int IssueId { get; set; }
    public Issue? Issue { get; set; }

    [Range(1, 5)]
    public int Stars { get; set; }

    [MaxLength(800)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
