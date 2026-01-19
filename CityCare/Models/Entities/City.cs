using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class City
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = "";

    [Required, MaxLength(10)]
    public string Code { get; set; } = "";   // KDY

    public bool IsActive { get; set; } = true;
}
