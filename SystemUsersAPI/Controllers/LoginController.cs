using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class LoginRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<ActionResult<SystemUser>> Login(LoginRequest request)
        {
            var user = await _context.SystemUsers
                .FirstOrDefaultAsync(u => u.Username == request.Username && 
                                        u.Password == request.Password &&
                                        u.IsActive);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Remove sensitive data before returning
            user.Password = "";
            
            return Ok(new { 
                message = "Login successful",
                user = user
            });
        }
    }
}