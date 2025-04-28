using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityPlannerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityPlannerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ActivityPlanner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityPlanner>>> GetActivityPlanners()
        {
            return await _context.ActivityPlanner
                .Include(a => a.Course)
                .Include(a => a.Activity)
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        // GET: api/ActivityPlanner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityPlanner>> GetActivityPlanner(int id)
        {
            var activityPlanner = await _context.ActivityPlanner
                .Include(a => a.Course)
                .Include(a => a.Activity)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

            if (activityPlanner == null)
            {
                return NotFound();
            }

            return activityPlanner;
        }

        // POST: api/ActivityPlanner
        [HttpPost]
        public async Task<ActionResult<ActivityPlanner>> CreateActivityPlanner(ActivityPlanner activityPlanner)
        {
            if (string.IsNullOrWhiteSpace(activityPlanner.Instructor) ||
                string.IsNullOrWhiteSpace(activityPlanner.CreatedBy))
            {
                return BadRequest("All required fields must be provided");
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == activityPlanner.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == activityPlanner.ActivityId);

            if (!courseExists || !activityExists)
            {
                return BadRequest("Invalid CourseId or ActivityId");
            }

            activityPlanner.CreatedDate = DateTime.UtcNow;
            activityPlanner.IsActive = true;

            _context.ActivityPlanner.Add(activityPlanner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivityPlanner), new { id = activityPlanner.Id }, activityPlanner);
        }

        // PUT: api/ActivityPlanner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivityPlanner(int id, ActivityPlanner activityPlanner)
        {
            if (id != activityPlanner.Id)
            {
                return BadRequest();
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == activityPlanner.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == activityPlanner.ActivityId);

            if (!courseExists || !activityExists)
            {
                return BadRequest("Invalid CourseId or ActivityId");
            }

            activityPlanner.ModifiedDate = DateTime.UtcNow;
            _context.Entry(activityPlanner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityPlannerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/ActivityPlanner/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityPlanner(int id)
        {
            var activityPlanner = await _context.ActivityPlanner.FindAsync(id);
            if (activityPlanner == null)
            {
                return NotFound();
            }

            activityPlanner.IsActive = false;
            activityPlanner.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityPlannerExists(int id)
        {
            return _context.ActivityPlanner.Any(e => e.Id == id);
        }
    }
}