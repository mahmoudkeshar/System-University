using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using system_university.Models;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        // إنشاء claims الأساسية  
        var claims = new List<Claim>
       {
           new Claim(ClaimTypes.NameIdentifier, user.Id),
           new Claim(ClaimTypes.Email, user.Email),
           new Claim(ClaimTypes.Name, user.UserName),
           new Claim(ClaimTypes.Role, user.GetType().Name)
       };

        // إضافة بيانات إضافية بناءً على نوع المستخدم  
        if (user is Student student)
        {
            claims.Add(new Claim("FullName", student.FullName));
            claims.Add(new Claim("StudentCode", student.StudentCode.ToString()));
        }
        else if (user is Instructor) // Fix: Removed unnecessary variable assignment  
        {
            // Add specific claims for Instractor if needed  
        }
        else if (user is Admin) // Fix: Removed unnecessary variable assignment  
        {
            // Add specific claims for Admin if needed  
        }

        // إنشاء المفتاح لتوقيع التوكن  
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // إنشاء التوكن  
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
            signingCredentials: creds
        );

        // إرجاع التوكن كـ string  
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
