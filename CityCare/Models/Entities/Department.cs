using System.ComponentModel.DataAnnotations;

namespace CityCare.Models.Entities;

public class Department
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = "";

    [Required, MaxLength(10)]
    public string Code { get; set; } = "";   // WTR

    public bool IsActive { get; set; } = true;

    // ✅ STEP 2: One Department → Many Issues
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
