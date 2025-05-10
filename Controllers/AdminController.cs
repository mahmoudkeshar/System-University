using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using system_university.Models;

namespace system_university.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;


        public AdminController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // Register Instructor**
        [HttpPost("AddInstructor")]
        public async Task<IActionResult> RegisterInstructor([FromBody] RegisterInsDTO model)
        {
            var user = new Instructor
            {
                UserName = model.Email,
                Email = model.Email,
                Role = Roles.Instructor
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Instructor);

                if (model.Subjects != null && model.Subjects.Any())
                {

                    var subjects = await _context.Subjects
                                                 .Where(s => model.Subjects.Contains(s.Name))
                                                 .ToListAsync();

                    if (subjects.Count != model.Subjects.Count)
                    {
                        var notFoundSubjects = model.Subjects.Except(subjects.Select(s => s.Name)).ToList();
                        return NotFound(new { Message = "Some subjects were not found", Subjects = notFoundSubjects });
                    }

                    user.Subjects = subjects;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Message = "Instructor registered successfully", Role = user.Role });
            }

            return BadRequest(result.Errors);
        }

        // Get All Subjects
        [HttpGet("GetAllSubjects")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _context.Subjects.ToListAsync();
            if (subjects == null || !subjects.Any())
            {
                return NotFound(new { Message = "No subjects found" });
            }
            return Ok(subjects);
        }


    }
}
