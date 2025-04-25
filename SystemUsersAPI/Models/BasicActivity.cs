namespace SystemUsersAPI.Models;

public class BasicActivity
{
    public int Id { get; set; }
    public required string ActivityName { get; set; } = string.Empty;
    public required bool IsActive { get; set; }
    public required string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}