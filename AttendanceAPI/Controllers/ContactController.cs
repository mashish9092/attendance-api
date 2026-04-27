using Microsoft.AspNetCore.Mvc;
using AttendanceAPI.Data;
using AttendanceAPI.Models;
using System.Data.SqlClient;

namespace AttendanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Save(ContactModel model)
        {
            _context.ContactMessages.Add(model);
            _context.SaveChanges();

            return Ok(new { message = "Saved Successfully" });
        }

        [HttpGet]
        public IActionResult GetContacts()
        {
            var data = _context.ContactMessages
                .Where(x => !x.IsDeleted)
                .ToList();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var contact = _context.ContactMessages.Find(id);

            if (contact == null)
                return NotFound();

            contact.IsDeleted = true;

            _context.SaveChanges();

            return Ok();
        }
    }
}