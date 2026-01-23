using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.ViewModels;

public class CreateStaffCodeViewModel
{
    [Required]
    public int CityId { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    [Range(2020, 2100)]
    public int Year { get; set; }

    [MaxLength(20)]
    public string? StaffPhone { get; set; }
}