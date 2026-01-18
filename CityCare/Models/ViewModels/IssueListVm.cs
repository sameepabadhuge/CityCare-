namespace CityCare.Models.ViewModels;

public class IssueListVm
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = "";
}
