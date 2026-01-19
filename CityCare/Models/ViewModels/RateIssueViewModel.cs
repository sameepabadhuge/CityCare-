using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.ViewModels;

public class RateIssueViewModel
{
    public int IssueId { get; set; }

    [Range(1, 5)]
    public int Stars { get; set; } = 5;

    [MaxLength(800)]
    public string? Comment { get; set; }
}
