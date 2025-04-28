using System;
using System.ComponentModel.DataAnnotations;

namespace SystemUsersAPI.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The Activity field is required.")]
        public int ActivityId { get; set; }
        [Required(ErrorMessage = "The Course field is required.")]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "The Trainee field is required.")]
        public int TraineeId { get; set; }
        [Required(ErrorMessage = "The Status field is required.")]
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public virtual BasicActivity? Activity { get; set; }
        public virtual Course? Course { get; set; }
        public virtual Trainee? Trainee { get; set; }
    }
}