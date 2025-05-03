using Microsoft.AspNetCore.Identity;

namespace system_university.Models
{
    public class Student : IdentityUser
    {
        public string FullName { get; set; }
        public int StudentCode { get; set; } // Fix: Corrected the property name to match the usage in AuthController  
    }
}
