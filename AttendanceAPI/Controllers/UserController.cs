using Microsoft.AspNetCore.Mvc;
using AttendanceAPI.Data;
using AttendanceAPI.Models;
using System.Linq;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL USERS
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        // ✅ GET BY ID (optional but useful)
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // ✅ ADD USER
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (user == null)
                return BadRequest();

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user); // 🔥 return object (not string)
        }
        [HttpPut("{id}")]
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

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            // 🔥 attendance delete first
            var records = _context.Attendance.Where(x => x.UserId == id).ToList();
            _context.Attendance.RemoveRange(records);

            // 🔥 then user delete
            _context.Users.Remove(user);

            _context.SaveChanges();

            return Ok();
        }

    }
}