using Microsoft.EntityFrameworkCore;
using AttendanceAPI.Models;
namespace AttendanceAPI.Data;

using AttendanceAPI.Data;
using AttendanceAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Attendance> Attendance { get; set; }

    // 🔥 ADD THIS
    public DbSet<ContactModel> ContactMessages { get; set; }
}