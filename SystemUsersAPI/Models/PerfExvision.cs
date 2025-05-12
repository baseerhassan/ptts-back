using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemUsersAPI.Models
{
    [Table("PERF_EXVISION")]
    public class PerfExvision
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        
        [Required]
        public int TraineeId { get; set; }
        
        public DateTime? CreatedTime { get; set; }
        
        [Required]
        [Column("ExVision_Shade_Id")]
        public int ExVisionShadeId { get; set; }
        
        [Column("TOPIC")]
        [StringLength(100)]
        public string Topic { get; set; }
        
        [Required]
        [Column("Scores")]
        public int Scores { get; set; }
        
        [Column("Observation")]
        [StringLength(100)]
        public string Observation { get; set; }
        
    }
}