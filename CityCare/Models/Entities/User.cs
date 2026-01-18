using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class User
{
    public int Id { get; set; }  // ✅ Primary Key

    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [Required, MaxLength(120)]
    public string Email { get; set; } = "";

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required, MaxLength(30)]
    public string Role { get; set; } = "Citizen"; // Citizen / Staff / Admin

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
