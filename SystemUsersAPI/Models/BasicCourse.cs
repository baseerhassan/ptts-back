namespace SystemUsersAPI.Models;

public class BasicCourse
{
    public int Id { get; set; }
    public required string CourseName { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
    public required string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}