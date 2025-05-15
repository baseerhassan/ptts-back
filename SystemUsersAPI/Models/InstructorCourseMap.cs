using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SystemUsersAPI.Models
{
    [Table("InstrCourseMap")]
    public class InstructorCourseMap
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SystemUserId { get; set; }
        public DateTime CreatedTime { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual SystemUser SystemUser { get; set; }
    }
}