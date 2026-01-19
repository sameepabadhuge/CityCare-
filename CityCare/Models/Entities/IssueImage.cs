using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class IssueImage
{
    public int Id { get; set; }

    public int IssueId { get; set; }
    public Issue? Issue { get; set; }

    [Required, MaxLength(400)]
    public string ImageUrl { get; set; } = "";
}
