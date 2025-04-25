using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            return await _context.Course.ToListAsync();
        }

        // GET: api/Course/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);

            if (course == null )
            {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Course/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            if (await _context.Course.AnyAsync(c => c.CourseName == course.CourseName && c.CourseId != id ))
            {
                return BadRequest("Course name must be unique");
            }

            if (course.FromDate > course.ToDate)
            {
                return BadRequest("From date cannot be after to date");
            }

            course.ModifiedDate = DateTime.UtcNow;
            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/Course
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            if (await _context.Course.AnyAsync(c => c.CourseName == course.CourseName && c.IsActive != false))
            {
                return BadRequest("Course name must be unique");
            }

            if (course.FromDate > course.ToDate)
            {
                return BadRequest("From date cannot be after to date");
            }

            course.CreatedDate = DateTime.UtcNow;
            course.ModifiedDate = DateTime.UtcNow;
            course.IsActive = true;

            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        }

        // DELETE: api/Course/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course == null || course.IsActive == false)
            {
                return NotFound();
            }

            course.IsActive = false;
            course.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}