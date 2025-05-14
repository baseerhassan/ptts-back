using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfCourseCritiqController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfCourseCritiqController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PerfCourseCritiq/course/{courseId}/trainee/{traineeId}
        [HttpGet("course/{courseId}/trainee/{traineeId}")]
        public async Task<ActionResult<PerfCourseCritiq>> GetByCourseAndTrainee(int courseId, int traineeId)
        {
            var critiq = await _context.PerfCourseCritiqs
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TraineeId == traineeId);

            if (critiq == null)
            {
                return NotFound();
            }

            return critiq;
        }

        // POST: api/PerfCourseCritiq
        [HttpPost]
        public async Task<ActionResult<PerfCourseCritiq>> PostPerfCourseCritiq(PerfCourseCritiq critiq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PerfCourseCritiqs.Add(critiq);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByCourseAndTrainee), 
                new { courseId = critiq.CourseId, traineeId = critiq.TraineeId }, critiq);
        }

        // PUT: api/PerfCourseCritiq/course/{courseId}/trainee/{traineeId}
        [HttpPut("course/{courseId}/trainee/{traineeId}")]
        public async Task<IActionResult> PutPerfCourseCritiq(int courseId, int traineeId, PerfCourseCritiq critiq)
        {
            if (courseId != critiq.CourseId || traineeId != critiq.TraineeId)
            {
                return BadRequest();
            }

            var existingCritiq = await _context.PerfCourseCritiqs
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.TraineeId == traineeId);

            if (existingCritiq == null)
            {
                return NotFound();
            }

            existingCritiq.q1 = critiq.q1;
            existingCritiq.q2 = critiq.q2;
            existingCritiq.q3 = critiq.q3;
            existingCritiq.q4 = critiq.q4;
            existingCritiq.q5 = critiq.q5;
            existingCritiq.q6 = critiq.q6;
            existingCritiq.q7 = critiq.q7;
            existingCritiq.q8 = critiq.q8;
            existingCritiq.q9 = critiq.q9;
            existingCritiq.q10 = critiq.q10;
            existingCritiq.q11 = critiq.q11;
            existingCritiq.q12 = critiq.q12;
            existingCritiq.Remarks = critiq.Remarks;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerfCourseCritiqExists(courseId, traineeId))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        private bool PerfCourseCritiqExists(int courseId, int traineeId)
        {
            return _context.PerfCourseCritiqs.Any(c => c.CourseId == courseId && c.TraineeId == traineeId);
        }
    }
}