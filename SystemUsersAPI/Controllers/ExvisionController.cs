using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExvisionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExvisionController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Shade Endpoints
        // GET: api/Exvision/shades
        [HttpGet("shades")]
        public async Task<ActionResult<IEnumerable<LookupExVisionShade>>> GetShades()
        {
            return await _context.LookupExVisionShades.ToListAsync();
        }

        // GET: api/Exvision/shades/5
        [HttpGet("shades/{id}")]
        public async Task<ActionResult<LookupExVisionShade>> GetShade(int id)
        {
            var shade = await _context.LookupExVisionShades.FindAsync(id);

            if (shade == null)
            {
                return NotFound();
            }

            return shade;
        }

        // GET: api/Exvision/shades/active
        [HttpGet("shades/active")]
        public async Task<ActionResult<IEnumerable<LookupExVisionShade>>> GetActiveShades()
        {
            return await _context.LookupExVisionShades
                .Where(s => s.IsActive == 1)
                .ToListAsync();
        }
        #endregion

        #region PerfExvision CRUD Endpoints
        // GET: api/Exvision
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfExvision>>> GetPerfExvisions()
        {
            return await _context.PerfExvisions
                .Include(p => p.CourseId)
                .Include(p => p.TraineeId)
                .Include(p => p.ExVisionShadeId)
                .ToListAsync();
        }

        // GET: api/Exvision/5
        [HttpGet("trainee")]
        public async Task<ActionResult<IEnumerable<PerfExvision>>> GetPerfExvision([FromQuery] int courseid, [FromQuery] int traineeid)
        {
            var perfExvision = await _context.PerfExvisions
                .Where(p => p.CourseId == courseid && p.TraineeId == traineeid)
                .ToListAsync();

            if (perfExvision == null)
            {
                return NotFound();
            }

            return perfExvision;
        }

        // POST: api/Exvision
        [HttpPost]
        public async Task<ActionResult<PerfExvision>> CreatePerfExvision(PerfExvision perfExvision)
        {
            // Set creation time if not provided
            if (perfExvision.CreatedTime == null)
            {
                perfExvision.CreatedTime = DateTime.Now;
            }

            // Validate foreign keys
            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == perfExvision.CourseId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == perfExvision.TraineeId);
            var shadeExists = await _context.LookupExVisionShades.AnyAsync(s => s.Id == perfExvision.ExVisionShadeId);

            if (!courseExists || !traineeExists || !shadeExists)
            {
                return BadRequest("Invalid CourseId, TraineeId, or ExVision_Shade_Id");
            }

            _context.PerfExvisions.Add(perfExvision);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerfExvision), new { id = perfExvision.Id }, perfExvision);
        }

        // PUT: api/Exvision/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerfExvision(int id, PerfExvision perfExvision)
        {
            if (id != perfExvision.Id)
            {
                return BadRequest();
            }

            // Validate foreign keys
            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == perfExvision.CourseId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == perfExvision.TraineeId);
            var shadeExists = await _context.LookupExVisionShades.AnyAsync(s => s.Id == perfExvision.ExVisionShadeId);

            if (!courseExists || !traineeExists || !shadeExists)
            {
                return BadRequest("Invalid CourseId, TraineeId, or ExVision_Shade_Id");
            }

            _context.Entry(perfExvision).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerfExvisionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Exvision/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfExvision(int id)
        {
            var perfExvision = await _context.PerfExvisions.FindAsync(id);
            if (perfExvision == null)
            {
                return NotFound();
            }

            _context.PerfExvisions.Remove(perfExvision);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PerfExvisionExists(int id)
        {
            return _context.PerfExvisions.Any(e => e.Id == id);
        }
        #endregion
    }
}