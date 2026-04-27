using Microsoft.AspNetCore.Mvc;
using AttendanceAPI.Data;
using AttendanceAPI.Models;
using System.Linq;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL USERS
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost("add-user")]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is null");

            if (string.IsNullOrEmpty(user.Email))
                return BadRequest("Email is required");

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
                return BadRequest("User already exists");

            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "User";

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User Added Successfully" });
        }
        // DELETE USER
        [HttpDelete("delete-user/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            // FIRST DELETE ATTENDANCE
            var attendance = _context.Attendance
                .Where(a => a.UserId == id)
                .ToList();

            _context.Attendance.RemoveRange(attendance);

            // 🔥 THEN DELETE USER
            _context.Users.Remove(user);

            _context.SaveChanges();

            return Ok(new { message = "User Deleted" });
        }
        [HttpPut("update-user/{id}")]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role;

            _context.SaveChanges();

            return Ok(new { message = "User Updated" });
        }
    }
}