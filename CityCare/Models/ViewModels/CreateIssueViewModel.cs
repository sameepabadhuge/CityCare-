using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.ViewModels;

public class CreateIssueViewModel
{
    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required, MaxLength(2000)]
    public string Description { get; set; } = "";

    [Required]
    public string Category { get; set; } = ""; // Water, Garbage

    [Required]
    public int CityId { get; set; }

    [Required, MaxLength(200)]
    public string LocationText { get; set; } = "";

    // Optional image upload (one image for now)
    public IFormFile? ImageFile { get; set; }
}
