using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BasicActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BasicActivity
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasicActivity>>> GetBasicActivity()
        {
            return await _context.BasicActivity.ToListAsync();
        }

        // GET: api/BasicActivity/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BasicActivity>> GetBasicActivity(int id)
        {
            var activity = await _context.BasicActivity.FindAsync(id);

            if (activity == null )
            {
                return NotFound();
            }

            return activity;
        }

        // POST: api/BasicActivity
        [HttpPost]
        public async Task<ActionResult<BasicActivity>> CreateBasicActivity(BasicActivity activity)
        {
            if (await _context.BasicActivity.AnyAsync(c => c.ActivityName == activity.ActivityName && c.IsActive))
            {
                return BadRequest("activity name must be unique");
            }

            activity.CreatedDate = DateTime.UtcNow;
            activity.IsActive = true;

            _context.BasicActivity.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBasicActivity), new { id = activity.Id }, activity);
        }

        // PUT: api/BasicActivity/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasicActivity(int id, BasicActivity activity)
        {
            if (id != activity.Id)
            {
                return BadRequest();
            }

            if (await _context.BasicActivity.AnyAsync(c => c.ActivityName == activity.ActivityName && c.Id != id && c.IsActive))
            {
                return BadRequest("activity name must be unique");
            }

            activity.ModifiedDate = DateTime.UtcNow;
            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BasicActivityExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/BasicActivity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasicActivity(int id)
        {
            var activity = await _context.BasicActivity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            activity.IsActive = false;
            activity.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BasicActivityExists(int id)
        {
            return _context.BasicActivity.Any(e => e.Id == id);
        }
    }
}