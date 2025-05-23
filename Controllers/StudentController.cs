﻿using iTextSharp.text.pdf.qrcode;
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
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public StudentController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // get all students
        [HttpGet("AllStudent")]
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
            var student = await _userManager.Users
                .OfType<Student>()
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
                return NotFound("Student not found");

            var qrText = $"Name: {student.FullName}, Code: {student.StudentCode}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var imageBytes = qrCode.GetGraphic(20);

            return File(imageBytes, "image/png");
        }

    }
}
