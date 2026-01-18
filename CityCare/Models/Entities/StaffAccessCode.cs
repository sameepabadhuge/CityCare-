using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class StaffAccessCode
{
    public int Id { get; set; } // ✅ Primary Key

    [Required, MaxLength(40)]
    public string Code { get; set; } = ""; // e.g., CITYCARE-STAFF-2026

    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UsedAt { get; set; }

    // Optional: who used it
    public int? UsedByUserId { get; set; }
    public User? UsedByUser { get; set; }
}
