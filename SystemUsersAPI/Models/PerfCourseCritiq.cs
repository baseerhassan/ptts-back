using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SystemUsersAPI.Models
{
    [Table("Perf_Course_Critiq")]
    public class PerfCourseCritiq
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int TraineeId { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public int? q1 { get; set; }
        public int? q2 { get; set; }
        public int? q3 { get; set; }
        public int? q4 { get; set; }
        public int? q5 { get; set; }
        public int? q6 { get; set; }
        public int? q7 { get; set; }
        public int? q8 { get; set; }
        public string? q9 { get; set; }
        public string? q10 { get; set; }
        public string? q11 { get; set; }
        public string? q12 { get; set; }
        public string? Remarks { get; set; }
        

    }
}