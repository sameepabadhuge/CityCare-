using CityCare.Models.Entities;

namespace CityCare.Models.ViewModels;

public class IssueDetailsViewModel
{
    public Issue Issue { get; set; } = new();

    public bool CanRate { get; set; } = false;

    public int? ExistingStars { get; set; }
    public string? ExistingComment { get; set; }

    public RateIssueViewModel RateVm { get; set; } = new();
}
