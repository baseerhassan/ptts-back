using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SystemUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SystemUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemUser>>> GetSystemUsers()
        {
            return await _context.SystemUsers.ToListAsync();
        }

        // GET: api/SystemUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemUser>> GetSystemUser(int id)
        {
            var systemUser = await _context.SystemUsers.FindAsync(id);

            if (systemUser == null)
            {
                return NotFound();
            }

            return systemUser;
        }

        // PUT: api/SystemUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSystemUser(int id, SystemUser systemUser)
        {
            if (id != systemUser.Id)
            {
                return BadRequest();
            }

            systemUser.ModifiedDate = DateTime.UtcNow;
            _context.Entry(systemUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemUserExists(id))
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

        // POST: api/SystemUsers
        [HttpPost]
        public async Task<ActionResult<SystemUser>> PostSystemUser(SystemUser systemUser)
        {
            systemUser.CreatedDate = DateTime.UtcNow;
            systemUser.ModifiedDate = DateTime.UtcNow;

            _context.SystemUsers.Add(systemUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSystemUser", new { id = systemUser.Id }, systemUser);
        }

        // DELETE: api/SystemUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSystemUser(int id)
        {
            var systemUser = await _context.SystemUsers.FindAsync(id);
            if (systemUser == null)
            {
                return NotFound();
            }

            _context.SystemUsers.Remove(systemUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SystemUserExists(int id)
        {
            return _context.SystemUsers.Any(e => e.Id == id);
        }
    }
}