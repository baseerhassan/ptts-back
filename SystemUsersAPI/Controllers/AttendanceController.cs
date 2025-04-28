using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendances()
        {
            return await _context.Attendance
                .Include(a => a.Course)
                .Include(a => a.Activity)
                .Include(a => a.Trainee)
                .ToListAsync();
        }

        [HttpGet("attendanceByCourseAndActivityId")]
       public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance(int courseId, int activityId)
{
    var attendances = await _context.Attendance
        .Include(a => a.Course)
        .Include(a => a.Activity)
        .Include(a => a.Trainee)
        .Where(a => a.CourseId == courseId && a.ActivityId == activityId)
        .ToListAsync();

    if (attendances == null || !attendances.Any())
    {
        return NotFound();
    }

    return attendances;
}
        // GET: api/Attendance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetAttendance(int id)
        {
            var attendance = await _context.Attendance
                .Include(a => a.Course)
                .Include(a => a.Activity)
                .Include(a => a.Trainee)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return attendance;
        }

        // POST: api/Attendance
        [HttpPost]
        public async Task<ActionResult<Attendance>> CreateAttendance(Attendance attendance)
        {
            if (string.IsNullOrWhiteSpace(attendance.Status) ||
                string.IsNullOrWhiteSpace(attendance.CreatedBy))
            {
                return BadRequest("All required fields must be provided");
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == attendance.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == attendance.ActivityId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == attendance.TraineeId);

            if (!courseExists || !activityExists || !traineeExists)
            {
                return BadRequest("Invalid CourseId, ActivityId, or TraineeId");
            }

            attendance.CreatedDate = DateTime.UtcNow;

            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, attendance);
        }

        // POST: api/Attendance/batch
        [HttpPost("batch")]
        public async Task<IActionResult> CreateAttendances([FromBody] List<Attendance> attendances)
        {
            if (attendances == null )
            {
                return BadRequest("Attendance list is empty or null.");
            }

            foreach (var attendance in attendances)
            {
                if (string.IsNullOrWhiteSpace(attendance.Status) ||
                    string.IsNullOrWhiteSpace(attendance.CreatedBy))
                {
                    return BadRequest("All required fields must be provided for each attendance.");
                }

                var courseExists = await _context.Course.AnyAsync(c => c.CourseId == attendance.CourseId);
                var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == attendance.ActivityId);
                var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == attendance.TraineeId);

                if (!courseExists || !activityExists || !traineeExists)
                {
                    return BadRequest("Invalid CourseId, ActivityId, or TraineeId for one or more attendances.");
                }

                attendance.CreatedDate = DateTime.UtcNow;
                _context.Attendance.Add(attendance);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok();
        }

        // PUT: api/Attendance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return BadRequest();
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == attendance.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == attendance.ActivityId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == attendance.TraineeId);

            if (!courseExists || !activityExists || !traineeExists)
            {
                return BadRequest("Invalid CourseId, ActivityId, or TraineeId");
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Attendance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendance.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendance.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendance.Any(e => e.Id == id);
        }
    }
}