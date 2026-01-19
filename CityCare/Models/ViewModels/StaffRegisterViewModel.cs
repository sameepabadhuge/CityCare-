using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.ViewModels;

public class StaffRegisterViewModel
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = "";

    // ✅ Staff chooses City + Department
    [Required]
    public int CityId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    // ✅ Staff enters access code manually
    [Required, MaxLength(25)]
    public string StaffAccessCode { get; set; } = "";
}
