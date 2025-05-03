using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using system_university.Models;

namespace system_university.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;
        private readonly JwtService _jwtService; // Add JwtService as a dependency  

        public AuthController(UserManager<Student> userManager, SignInManager<Student> signInManager, JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService; // Initialize JwtService  
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            // التحقق من أن الكود 6 أرقام
            if (model.StudentCode < 100000 || model.StudentCode > 999999)
                return BadRequest(new { Error = "StudentCode must be exactly 6 digits." });

            // Fixing the lambda expression to use '==' for comparison instead of '=' for assignment
            var isCodeExists = await _userManager.Users.AnyAsync(u => u.StudentCode == model.StudentCode);
            if (isCodeExists)
                return BadRequest(new { Error = "StudentCode already exists." });

            var user = new Student
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                StudentCode = model.StudentCode
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(new { Message = "Registration successful" });

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user); // Use the instance of JwtService  

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName,
                    user.StudentCode
                }
            });
        }
    }
}
