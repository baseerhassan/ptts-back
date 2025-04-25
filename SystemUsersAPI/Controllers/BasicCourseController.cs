using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicCourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BasicCourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BasicCourse
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasicCourse>>> GetBasicCourses()
        {
            return await _context.BasicCourse.ToListAsync();
        }

        // GET: api/BasicCourse/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BasicCourse>> GetBasicCourse(int id)
        {
            var course = await _context.BasicCourse.FindAsync(id);

            if (course == null )
            {
                return NotFound();
            }

            return course;
        }

        // POST: api/BasicCourse
        [HttpPost]
        public async Task<ActionResult<BasicCourse>> CreateBasicCourse(BasicCourse course)
        {
            if (await _context.BasicCourse.AnyAsync(c => c.CourseName == course.CourseName && c.IsActive))
            {
                return BadRequest("Course name must be unique");
            }

            course.CreatedDate = DateTime.UtcNow;
            course.IsActive = true;

            _context.BasicCourse.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBasicCourse), new { id = course.Id }, course);
        }

        // PUT: api/BasicCourse/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasicCourse(int id, BasicCourse course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            if (await _context.BasicCourse.AnyAsync(c => c.CourseName == course.CourseName && c.Id != id && c.IsActive))
            {
                return BadRequest("Course name must be unique");
            }

            course.ModifiedDate = DateTime.UtcNow;
            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BasicCourseExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/BasicCourse/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasicCourse(int id)
        {
            var course = await _context.BasicCourse.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            course.IsActive = false;
            course.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BasicCourseExists(int id)
        {
            return _context.BasicCourse.Any(e => e.Id == id);
        }
    }
}