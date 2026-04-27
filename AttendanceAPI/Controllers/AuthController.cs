using Microsoft.AspNetCore.Mvc;
using AttendanceAPI.Data;
using AttendanceAPI.Models;
using System.Linq;
using System;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginUser)
        {
            if (loginUser == null)
                return BadRequest("Invalid data");

            var user = _context.Users
                .FirstOrDefault(x => x.Email == loginUser.Email && x.Password == loginUser.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                userId = user.UserId,
                email = user.Email,
                name=user.Name,
                role = user.Role
            });
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(x => x.Email == user.Email);

                if (existingUser != null)
                    return BadRequest(new { message = "User already exists ❗" });

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { message = "User registered successfully ✅" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}