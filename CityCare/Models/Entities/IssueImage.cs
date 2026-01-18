using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class IssueImage
{
    public int Id { get; set; } // ✅ Primary Key

    [Required, MaxLength(500)]
    public string ImageUrl { get; set; } = ""; // store Cloudinary URL or file path

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int IssueId { get; set; }
    public Issue? Issue { get; set; }
}
