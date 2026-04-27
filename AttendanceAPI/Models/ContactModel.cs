using System.ComponentModel.DataAnnotations;

namespace AttendanceAPI.Models
{
    public class ContactModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
    }
}
