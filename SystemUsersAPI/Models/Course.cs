namespace SystemUsersAPI.Models;
    public class Course
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; } = string.Empty;
        public required string CourseNo { get; set; } = string.Empty;
        public required bool? IsActive { get; set; }
        public required string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public required string? ModifiedBy { get; set; }
        public required DateTime? ModifiedDate { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? TotalOfficers { get; set; }
        public int? TotalAirmen { get; set; }
        public int? TotalCivilians { get; set; }
        public string? Remarks { get; set; }
         public string? TotalDuration { get; set; }
    }
