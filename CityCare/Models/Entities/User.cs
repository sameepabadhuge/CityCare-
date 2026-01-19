using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class User : IdentityUser
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    // ✅ Citizen fields
    [MaxLength(200)]
    public string? Address { get; set; }

    // ✅ Common: both can have City
    public int? CityId { get; set; }
    public City? City { get; set; }

    // ✅ Staff only
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
