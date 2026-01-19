using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.ViewModels;

public class CitizenRegisterViewModel
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";

    // ✅ Citizen needs address
    [Required, MaxLength(200)]
    public string Address { get; set; } = "";

    // ✅ Citizen chooses city (default later in report page)
    [Required]
    public int CityId { get; set; }
}
