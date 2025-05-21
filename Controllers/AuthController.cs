using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using system_university.Models;

namespace system_university.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JwtService jwtService , IConfiguration configuration , AppDbContext context) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _configuration = configuration;
            _context = context;
        }

        // 📝 **Register Student**
        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterDTO model)
        {
            // التحقق من أن الكود 6 أرقام  
            if (model.StudentCode < 100000 || model.StudentCode > 999999)
                return BadRequest(new { Error = "StudentCode must be exactly 6 digits." });

            // التحقق من وجود الكود مسبقًا
            var isCodeExists = await _userManager.Users
                .OfType<Student>()
                .AnyAsync(u => u.StudentCode == model.StudentCode);

            if (isCodeExists)
                return BadRequest(new { Error = "StudentCode already exists." });

            // إنشاء الطالب
            var user = new Student
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                StudentCode = model.StudentCode,
                Role = Roles.Student
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
                return Ok(new { Message = "Student registration successful" });
            }

            return BadRequest(result.Errors);
        }

        // 📝 **Register Admin**
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDTO model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Role = Roles.Admin
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin);
                return Ok(new { Message = "Admin registered successfully", Role = user.Role });
            }

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password" });
          
            var token = await GenerateJwtToken(user);
         
            var userData = new
            {
                user.Id,
                user.Email,
                user.Role,
                Token = token
            };

            return Ok(userData);
        }


        // Fix for CS1061: 'AppDbContext' does not contain a definition for 'Id'  
        // The issue is that the `GenerateJwtToken` method is incorrectly using `AppDbContext` instead of the `User` entity.  
        // Update the method to use the correct `User` type.  

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
   {
       new Claim(JwtRegisteredClaimNames.Sub, user.Id),
       new Claim(JwtRegisteredClaimNames.Email, user.Email),
       new Claim(ClaimTypes.Name, user.UserName)
   };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // ✅ تعديل القراءة لتكون من Jwt:Key وليس Jwt:Secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default_secret"));

            // ✅ التأكد من طول المفتاح (للتشخيص فقط، يمكن إزالته لاحقًا)
            Console.WriteLine($"Key Length: {key.Key.Length * 8} bits"); // يجب أن تكون 256 bits أو أكثر

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
