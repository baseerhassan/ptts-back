using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    public class EvaluationBatchRequest
    {
        [Required(ErrorMessage = "The evaluations field is required.")]
        public List<Evaluation> Evaluations { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EvaluationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Evaluation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evaluation>>> GetEvaluations()
        {
            return await _context.Evaluation
                .Include(e => e.Course)
                .Include(e => e.Activity)
                .Include(e => e.Trainee)
                .ToListAsync();
        }

        // GET: api/Evaluation/byCourseAndActivity
        [HttpGet("byCourseAndActivity")]
        public async Task<ActionResult<IEnumerable<Evaluation>>> GetEvaluationsByCourseAndActivity(int courseId, int activityId)
        {
            var evaluations = await _context.Evaluation
                .Include(e => e.Course)
                .Include(e => e.Activity)
                .Include(e => e.Trainee)
                .Where(e => e.CourseId == courseId && e.ActivityId == activityId)
                .ToListAsync();

            if (evaluations == null || !evaluations.Any())
            {
                return NotFound();
            }

            return evaluations;
        }

        // GET: api/Evaluation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evaluation>> GetEvaluation(int id)
        {
            var evaluation = await _context.Evaluation
                .Include(e => e.Course)
                .Include(e => e.Activity)
                .Include(e => e.Trainee)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evaluation == null)
            {
                return NotFound();
            }

            return evaluation;
        }

        // POST: api/Evaluation
        [HttpPost]
        public async Task<ActionResult<Evaluation>> CreateEvaluation(Evaluation evaluation)
        {
            if (string.IsNullOrWhiteSpace(evaluation.Grade) ||
                string.IsNullOrWhiteSpace(evaluation.Status) ||
                string.IsNullOrWhiteSpace(evaluation.Remarks) ||
                string.IsNullOrWhiteSpace(evaluation.CreatedBy))
            {
                return BadRequest("All required fields must be provided");
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == evaluation.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == evaluation.ActivityId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == evaluation.TraineeId);

            if (!courseExists || !activityExists || !traineeExists)
            {
                return BadRequest("Invalid CourseId, ActivityId, or TraineeId");
            }

            evaluation.CreatedDate = DateTime.UtcNow;

            _context.Evaluation.Add(evaluation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvaluation), new { id = evaluation.Id }, evaluation);
        }

        // POST: api/Evaluation/batch
        [HttpPost("batch")]
        public async Task<IActionResult> CreateEvaluations([FromBody] EvaluationBatchRequest request)
        {
            if (request?.Evaluations == null)
            {
                return BadRequest("Evaluations list is empty or null.");
            }
            
            var evaluations = request.Evaluations;

            foreach (var evaluation in evaluations)
            {
                if (string.IsNullOrWhiteSpace(evaluation.Status) ||
                    string.IsNullOrWhiteSpace(evaluation.CreatedBy))
                {
                    return BadRequest("Status and CreatedBy fields must be provided for each evaluation.");
                }

                var courseExists = await _context.Course.AnyAsync(c => c.CourseId == evaluation.CourseId);
                var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == evaluation.ActivityId);
                var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == evaluation.TraineeId);

                if (!courseExists || !activityExists || !traineeExists)
                {
                    return BadRequest("Invalid CourseId, ActivityId, or TraineeId for one or more evaluations.");
                }

                // Check if evaluation already exists for this course, activity, and trainee
                var existingEvaluation = await _context.Evaluation
                    .FirstOrDefaultAsync(e => e.CourseId == evaluation.CourseId && 
                                         e.ActivityId == evaluation.ActivityId && 
                                         e.TraineeId == evaluation.TraineeId);

                if (existingEvaluation != null)
                {
                    // Update existing evaluation
                    existingEvaluation.Grade = evaluation.Grade;
                    existingEvaluation.Status = evaluation.Status;
                    existingEvaluation.Remarks = evaluation.Remarks;
                    existingEvaluation.CreatedBy = evaluation.CreatedBy;
                    existingEvaluation.CreatedDate = DateTime.UtcNow;
                    _context.Entry(existingEvaluation).State = EntityState.Modified;
                }
                else
                {
                    // Create new evaluation
                    evaluation.CreatedDate = DateTime.UtcNow;
                    _context.Evaluation.Add(evaluation);
                }
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

        // PUT: api/Evaluation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvaluation(int id, Evaluation evaluation)
        {
            if (id != evaluation.Id)
            {
                return BadRequest();
            }

            var courseExists = await _context.Course.AnyAsync(c => c.CourseId == evaluation.CourseId);
            var activityExists = await _context.BasicActivity.AnyAsync(a => a.Id == evaluation.ActivityId);
            var traineeExists = await _context.Trainee.AnyAsync(t => t.Id == evaluation.TraineeId);

            if (!courseExists || !activityExists || !traineeExists)
            {
                return BadRequest("Invalid CourseId, ActivityId, or TraineeId");
            }

            _context.Entry(evaluation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Evaluation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(int id)
        {
            var evaluation = await _context.Evaluation.FindAsync(id);
            if (evaluation == null)
            {
                return NotFound();
            }

            _context.Evaluation.Remove(evaluation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EvaluationExists(int id)
        {
            return _context.Evaluation.Any(e => e.Id == id);
        }
    }
}