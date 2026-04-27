using Microsoft.AspNetCore.Mvc;
using AttendanceAPI.Data;
using AttendanceAPI.Models;
using System;
using System.Linq;

namespace AttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // ================================
        // ✅ CHECK-IN
        // ================================
        [HttpPost("checkin/{userId}")]
        public IActionResult CheckIn(int userId)
        {
            try
            {
                var today = DateTime.Today;

                var existing = _context.Attendance
                    .FirstOrDefault(x => x.UserId == userId && x.Date.Date == today);

                if (existing != null)
                    return BadRequest(new { message = "Already checked in today ❗" });

                var now = DateTime.Now;

                var attendance = new Attendance
                {
                    UserId = userId,
                    Date = today,
                    CheckInTime = now,
                    IsLate = now.TimeOfDay > new TimeSpan(9, 15, 0)
                };

                _context.Attendance.Add(attendance);
                _context.SaveChanges();

                return Ok(new { message = "Check-in successful ✅" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ================================
        // ✅ CHECK-OUT
        // ================================
        [HttpPost("checkout/{userId}")]
        public IActionResult CheckOut(int userId)
        {
            try
            {
                var today = DateTime.Today;

                var attendance = _context.Attendance
                    .FirstOrDefault(x => x.UserId == userId && x.Date.Date == today);

                if (attendance == null)
                    return BadRequest(new { message = "No check-in found ❗" });

                if (attendance.CheckOutTime != null)
                    return BadRequest(new { message = "Already checked out ❗" });

                attendance.CheckOutTime = DateTime.Now;

                _context.SaveChanges();

                return Ok(new { message = "Check-out successful ✅" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ================================
        // ✅ GET USER ATTENDANCE
        // ================================
        [HttpGet("user/{userId}")]
        public IActionResult GetAttendanceByUser(int userId)
        {
            try
            {
                var data = _context.Attendance
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.Date)
                    .Select(x => new
                    {
                        date = x.Date,
                        checkInTime = x.CheckInTime,
                        checkOutTime = x.CheckOutTime,
                        isLate = x.IsLate ?? false,
                        status = x.CheckOutTime != null ? "Completed" : "Present"
                    })
                    .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ================================
        // ✅ GET ALL (ADMIN / REPORT)
        // ================================
        [HttpGet("list")]
        public IActionResult GetAllAttendance()
        {
            try
            {
                var data = (from a in _context.Attendance
                            join u in _context.Users
                            on a.UserId equals u.UserId
                            select new
                            {
                                userId = a.UserId,
                                name = u.Name,
                                email = u.Email,
                                date = a.Date,
                                checkInTime = a.CheckInTime,
                                checkOutTime = a.CheckOutTime,
                                isLate = a.IsLate ?? false
                            })
                            .OrderByDescending(x => x.date)
                            .ToList();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ================================
        // ✅ MONTHLY CALENDAR API
        // ================================
        [HttpGet("month")]
        public IActionResult GetMonthlyAttendance(int userId, int month, int year)
        {
            try
            {
                var data = _context.Attendance
                    .Where(a => a.UserId == userId &&
                                a.Date.Month == month &&
                                a.Date.Year == year)
                    .ToList();

                var result = data.ToDictionary(
                    a => a.Date.Day,
                    a =>
                        a.CheckInTime == null ? "absent" :
                        a.IsLate == true ? "late" :
                        "present"
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}