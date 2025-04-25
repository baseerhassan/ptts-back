namespace SystemUsersAPI.Models;

public class Trainee
{
    public int Id { get; set; }
    public required string Arm { get; set; } = string.Empty;
    public required string Rank { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required string PakNo { get; set; } = string.Empty;
    public required string Gender { get; set; } = string.Empty;
    public required string RegistrationNo { get; set; } = string.Empty;
    public required string CourseId { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
    public required string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
     public string? CourseName { get; set; }
}