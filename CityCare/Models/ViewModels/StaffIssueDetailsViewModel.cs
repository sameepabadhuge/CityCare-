using CityCare.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.ViewModels;

public class StaffIssueDetailsViewModel
{
    public Issue Issue { get; set; } = new();

    [Required]
    public IssueStatus NewStatus { get; set; }
}
