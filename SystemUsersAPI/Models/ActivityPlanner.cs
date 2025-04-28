namespace SystemUsersAPI.Models;

public class ActivityPlanner
{
    public int Id { get; set; }
    public required DateOnly Date { get; set; }
    public string? Time { get; set; }
    public required int CourseId { get; set; }
    public required int ActivityId { get; set; }
    public string? Remarks { get; set; }
    public required string Instructor { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
    public required string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public Course? Course { get; set; }
    public BasicActivity? Activity { get; set; }
}