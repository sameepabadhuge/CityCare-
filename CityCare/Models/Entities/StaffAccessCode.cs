using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class StaffAccessCode
{
    public int Id { get; set; }

    [Required, MaxLength(25)]
    public string Code { get; set; } = "";  // CC-KDY-WTR-2026

    public int CityId { get; set; }
    public City? City { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int Year { get; set; } = DateTime.UtcNow.Year;
    public bool IsActive { get; set; } = true;

    // Optional: contact number for this staff group (for “suggest staff mobile”)
    [MaxLength(20)]
    public string? StaffPhone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
