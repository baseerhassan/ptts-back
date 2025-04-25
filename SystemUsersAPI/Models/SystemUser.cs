namespace SystemUsersAPI.Models;

public class SystemUser
{
    public int Id { get; set; }
    public required string Rank { get; set; }
    public required string Name { get; set; }
    public required string PakNo { get; set; }
    public required string Designation { get; set; }
    public required string Group { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
     public required bool IsActive { get; set; }
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

}