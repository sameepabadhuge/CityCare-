using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.DTOs;

public class RegisterUserDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = "";

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required, MaxLength(30)]
    public string Role { get; set; } = "Citizen";
}
