using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel;
using System.IO;
using system_university.Models;
using Excel = OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext; // تعريف اختصار لـ OfficeOpenXml

namespace system_university.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public InstructorController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("instructor/{instructorId}/subjects")]
        public async Task<IActionResult> GetInstructorSubjects(string instructorId)
        {
            var instructor = await _context.Instructors
                                           .Include(i => i.Subjects)
                                           .FirstOrDefaultAsync(i => i.Id == instructorId);

            if (instructor == null)
                return NotFound(new { message = "Instructor not found" });

            var subjects = instructor.Subjects.Select(s => new
            {
                s.Id,
                s.Name
            }).ToList();

            return Ok(subjects);
        }

        [HttpPost("save-scan-attend")]
        public async Task<IActionResult> SaveScan([FromBody] StudentAttendance model)
        {
            // Check if the student is already registered in attendance for the same section
            var existingAttendance = await _context.StudentAttendances
                                                   .FirstOrDefaultAsync(a => a.StudentCode == model.StudentCode && a.section == model.section);

            if (existingAttendance != null)
            {
                return Conflict(new { message = "Student attendance already saved for this section." });
            }

            // Add student to attendance
            _context.StudentAttendances.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Data saved successfully." });
        }


        [HttpGet("export-attendance")]
        public IActionResult ExportAttendance(int section)
        {
            // Fetch data from the database filtered by section
            var attendanceData = _context.StudentAttendances
                                          .Where(a => a.section == section)
                                          .OrderBy(a => a.Name)
                                          .ToList();

            if (!attendanceData.Any())
            {
                return NotFound(new { message = "No attendance data found for the specified section." });
            }

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Section_{section}");

                // Create headers in the first row
                worksheet.Cell(1, 1).Value = "Student Code";
                worksheet.Cell(1, 2).Value = "Student Name";
                worksheet.Cell(1, 3).Value = "Section";

                // Populate data in the table
                int row = 2;
                foreach (var attendance in attendanceData)
                {
                    worksheet.Cell(row, 1).Value = attendance.StudentCode;
                    worksheet.Cell(row, 2).Value = attendance.Name;
                    worksheet.Cell(row, 3).Value = attendance.section;
                    row++;
                }

                // Auto adjust column width
                worksheet.Columns().AdjustToContents();

                // Prepare the response to send as an Excel file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Attendance_Section_{section}.xlsx");
                }
            }
        }

        [HttpPost("save-scan-Quiz")]
        public async Task<IActionResult> ScanQuiz([FromBody] DegreeOfQuizes model)
        {
            // Check if the student degree is already saved for this quiz  
            var existingQuizDegree = await _context.degreeOfQuizes
                                                   .FirstOrDefaultAsync(a => a.StudentCode == model.StudentCode && a.QuizCode == model.QuizCode);

            if (existingQuizDegree != null)
            {
                return Conflict(new { message = "Student degree already saved for this quiz." });
            }

            // Add student quiz degree  
            _context.degreeOfQuizes.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Data saved successfully." });
        }


        [HttpGet("export-DegreeOfQuizes")]
        public IActionResult ExportDegree(int QuizCode)
        {
            // Fetch data from the database filtered by QuizCode  
            var quizData = _context.degreeOfQuizes
                                   .Where(a => a.QuizCode == QuizCode)
                                   .OrderBy(a => a.StudentName)
                                   .ToList();

            if (!quizData.Any())
            {
                return NotFound(new { message = "No quiz data found for the specified QuizCode." });
            }

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Quiz_{QuizCode}");

                // Create headers in the first row  
                worksheet.Cell(1, 1).Value = "Student Code";
                worksheet.Cell(1, 2).Value = "Student Name";
                worksheet.Cell(1, 3).Value = "Quiz Code";
                worksheet.Cell(1, 4).Value = "Degree";

                // Populate data in the table  
                int row = 2;
                foreach (var quiz in quizData)
                {
                    worksheet.Cell(row, 1).Value = quiz.StudentCode;
                    worksheet.Cell(row, 2).Value = quiz.StudentName;
                    worksheet.Cell(row, 3).Value = quiz.QuizCode;
                    worksheet.Cell(row, 4).Value = quiz.Degree;
                    row++;
                }

                // Auto adjust column width  
                worksheet.Columns().AdjustToContents();

                // Prepare the response to send as an Excel file  
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Quiz_{QuizCode}.xlsx");
                }
            }
        }
    }
}