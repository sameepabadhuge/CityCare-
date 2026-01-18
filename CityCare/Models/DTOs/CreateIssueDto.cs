using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.DTOs;

public class CreateIssueDto
{
    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required, MaxLength(1000)]
    public string Description { get; set; } = "";

    [Required, MaxLength(60)]
    public string Category { get; set; } = "General";

    [MaxLength(150)]
    public string? LocationText { get; set; }

    [Required]
    public int UserId { get; set; }
}
