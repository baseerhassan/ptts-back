using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorCourseMapController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InstructorCourseMapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/InstructorCourseMap
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstructorCourseMap>>> GetInstructorCourseMaps()
        {
            return await _context.InstructorCourseMaps
                
                .ToListAsync();
        }

        // GET: api/InstructorCourseMap/course/{courseId}
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<InstructorCourseMap>>> GetInstructorCourseMapsByCourseId(int courseId)
        {
            var mappings = await _context.InstructorCourseMaps
                .Where(m => m.CourseId == courseId)
                .ToListAsync();

            if (!mappings.Any())
            {
                return NotFound($"No instructor mappings found for course ID {courseId}");
            }

            return mappings;
        }

        // GET: api/InstructorCourseMap/5
      

        // POST: api/InstructorCourseMap
        [HttpPost]
        public async Task<ActionResult<InstructorCourseMap>> PostInstructorCourseMap([FromBody] object payload)
        {
            try
            {
                // Try to deserialize as a single object
                var singleMap = System.Text.Json.JsonSerializer.Deserialize<InstructorCourseMap>(payload.ToString());
                return await CreateSingleMapping(singleMap);
            }
            catch
            {
                try
                {
                    // Try to deserialize as a list
                    var mapList = System.Text.Json.JsonSerializer.Deserialize<List<InstructorCourseMap>>(payload.ToString());
                    return await CreateMultipleMappings(mapList);
                }
                catch
                {
                    return BadRequest("Invalid payload format");
                }
            }
        }

        private async Task<ActionResult<InstructorCourseMap>> CreateSingleMapping(InstructorCourseMap map)
        {
            if (!await ValidateMapping(map))
            {
                return BadRequest("Invalid CourseId or SystemUserId");
            }

            map.CreatedTime = DateTime.UtcNow;
            _context.InstructorCourseMaps.Add(map);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstructorCourseMaps), new { id = map.Id }, map);
        }

        private async Task<ActionResult<InstructorCourseMap>> CreateMultipleMappings(List<InstructorCourseMap> maps)
        {
            foreach (var map in maps)
            {
                if (!await ValidateMapping(map))
                {
                    return BadRequest($"Invalid CourseId {map.CourseId} or SystemUserId {map.SystemUserId}");
                }
                map.CreatedTime = DateTime.UtcNow;
            }

            _context.InstructorCourseMaps.AddRange(maps);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstructorCourseMaps), null, maps);
        }

        // PUT: api/InstructorCourseMap/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstructorCourseMap(int id, InstructorCourseMap instructorCourseMap)
        {
            if (id != instructorCourseMap.Id)
            {
                return BadRequest();
            }

            if (!await ValidateMapping(instructorCourseMap))
            {
                return BadRequest("Invalid CourseId or SystemUserId");
            }

            _context.Entry(instructorCourseMap).State = EntityState.Modified;
            _context.Entry(instructorCourseMap).Property(x => x.CreatedTime).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstructorCourseMapExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        private bool InstructorCourseMapExists(int id)
        {
            return _context.InstructorCourseMaps.Any(e => e.Id == id);
        }

        private async Task<bool> ValidateMapping(InstructorCourseMap map)
        {
            return await _context.Course.AnyAsync(c => c.CourseId == map.CourseId) &&
                   await _context.SystemUsers.AnyAsync(u => u.Id == map.SystemUserId);
        }
    }
}