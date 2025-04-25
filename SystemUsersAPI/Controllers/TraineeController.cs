using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TraineeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Trainee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainee>>> GetTrainees()
        {
            return await _context.Trainee.ToListAsync();
        }

        // GET: api/Trainee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Trainee>> GetTrainee(int id)
        {
            var trainee = await _context.Trainee.FindAsync(id);

            if (trainee == null )
            {
                return NotFound();
            }

            return trainee;
        }

        // POST: api/Trainee
        [HttpPost]
        public async Task<ActionResult<Trainee>> CreateTrainee(Trainee trainee)
        {
            if (string.IsNullOrWhiteSpace(trainee.Arm) || string.IsNullOrWhiteSpace(trainee.Rank) ||
                string.IsNullOrWhiteSpace(trainee.Name) || string.IsNullOrWhiteSpace(trainee.PakNo) ||
                string.IsNullOrWhiteSpace(trainee.Gender) || string.IsNullOrWhiteSpace(trainee.RegistrationNo) ||
                string.IsNullOrWhiteSpace(trainee.CourseId) || string.IsNullOrWhiteSpace(trainee.CreatedBy))
            {
                return BadRequest("All fields are required");
            }

            if (await _context.Trainee.AnyAsync(t => t.PakNo == trainee.PakNo && t.IsActive))
            {
                return BadRequest("PAK number must be unique");
            }

            trainee.CreatedDate = DateTime.UtcNow;
            trainee.IsActive = true;

            _context.Trainee.Add(trainee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrainee), new { id = trainee.Id }, trainee);
        }

        // PUT: api/Trainee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrainee(int id, Trainee trainee)
        {
            if (id != trainee.Id)
            {
                return BadRequest();
            }

            if (await _context.Trainee.AnyAsync(t => t.PakNo == trainee.PakNo && t.Id != id ))
            {
                return BadRequest("PAK number must be unique");
            }

            trainee.ModifiedDate = DateTime.UtcNow;
            _context.Entry(trainee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TraineeExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Trainee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            var trainee = await _context.Trainee.FindAsync(id);
            if (trainee == null)
            {
                return NotFound();
            }

            trainee.IsActive = false;
            trainee.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TraineeExists(int id)
        {
            return _context.Trainee.Any(e => e.Id == id);
        }
    }
}