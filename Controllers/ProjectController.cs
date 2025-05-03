using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using system_university.Models;

namespace system_university.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager; // Added UserManager field  

        public ProjectController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager  
        }

        // get all students
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Students = await _context.Students.ToListAsync();
            return Ok(Students);
        }

        // GET: api/Schedule/level/3  
        [HttpGet("level/{level}")]
        public async Task<IActionResult> GetByLevel(int level)
        {
            var schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Level == level);

            if (schedule == null)
                return NotFound(new { message = "Level not found" });

            return Ok(schedule);
        }

        [HttpGet("qrcode/{studentCode}")]
        public async Task<IActionResult> GetQRCode(int studentCode)
        {
            var student = await _userManager.Users.FirstOrDefaultAsync(s => s.StudentId == studentCode);
            if (student == null)
                return NotFound("Student not found");

            var qrText = $"Name: {student.FullName}, Code: {student.StudentId}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var imageBytes = qrCode.GetGraphic(20);

            return File(imageBytes, "image/png");
        }
    }
}
