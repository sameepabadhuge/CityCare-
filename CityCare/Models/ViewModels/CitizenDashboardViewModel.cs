using CityCare.Models.Entities;

namespace CityCare.Models.ViewModels;

public class CitizenDashboardViewModel
{
    public List<Issue> Issues { get; set; } = new();
}
