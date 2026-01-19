using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public enum IssueStatus { Pending = 0, InProgress = 1, Resolved = 2 }

public class Issue
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required, MaxLength(2000)]
    public string Description { get; set; } = "";

    [Required, MaxLength(40)]
    public string Category { get; set; } = ""; // Water, Garbage

    // citizen selects city (default = their registered city)
    public int CityId { get; set; }
    public City? City { get; set; }

    // location text (address/village)
    [Required, MaxLength(200)]
    public string LocationText { get; set; } = "";

    // map pin (optional for later google maps)
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public IssueStatus Status { get; set; } = IssueStatus.Pending;

    // Citizen
    [Required]
    public string CitizenId { get; set; } = "";
    public User? Citizen { get; set; }

    // Assigned staff (optional)
    public string? AssignedStaffId { get; set; }
    public User? AssignedStaff { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<IssueImage> Images { get; set; } = new List<IssueImage>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public Rating? Rating { get; set; }
}
