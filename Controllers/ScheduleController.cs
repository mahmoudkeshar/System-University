using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using system_university.Models;

namespace system_university.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScheduleController(AppDbContext context)
        {
            _context = context;
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
    }
}
