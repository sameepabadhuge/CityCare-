using CityCare.Models.Entities;

namespace CityCare.Models.ViewModels;

public class StaffDashboardViewModel
{
    public string Filter { get; set; } = "all";
    public List<Issue> Issues { get; set; } = new();
}
