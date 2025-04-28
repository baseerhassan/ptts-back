using Microsoft.AspNetCore.Mvc;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Attendance>> CreateAttendance([FromBody] Attendance attendance)
        {
            if (attendance == null)
            {
                return BadRequest("Invalid attendance data.");
            }

            if (string.IsNullOrWhiteSpace(attendance.Status))
            {
                return BadRequest("Status is required.");
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == attendance.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == attendance.ActivityId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == attendance.TraineeId);

            if (!courseExists || !activityExists || !traineeExists)
            {
                return BadRequest("Invalid CourseId, ActivityId, or TraineeId");
            }

            attendance.CreatedBy = "system";
            attendance.CreatedDate = DateTime.UtcNow;
            attendance.ModifiedBy = "system";
            attendance.ModifiedDate = DateTime.UtcNow;

            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateAttendance), new { id = attendance.Id }, attendance);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Attendance>>> CreateAttendances([FromBody] List<Attendance> attendances)
        {
            if (attendances == null || !attendances.Any())
            {
                return BadRequest("Invalid attendance data.");
            }

            foreach (var attendance in attendances)
            {
                if (attendance == null || string.IsNullOrWhiteSpace(attendance.Status))
                {
                    return BadRequest("Status is required for all attendance records.");
                }

                var courseExists = await _context.Course.AnyAsync(c => c.CourseId == attendance.CourseId);
                var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == attendance.ActivityId);
                var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == attendance.TraineeId);

                if (!courseExists || !activityExists || !traineeExists)
                {
                    return BadRequest($"Invalid CourseId, ActivityId, or TraineeId for attendance record.");
                }

                attendance.CreatedBy = "system";
                attendance.CreatedDate = DateTime.UtcNow;
                attendance.ModifiedBy = "system";
                attendance.ModifiedDate = DateTime.UtcNow;

                _context.Attendance.Add(attendance);
            }

            await _context.SaveChangesAsync();

            return Ok(attendances);
        }
    }
}