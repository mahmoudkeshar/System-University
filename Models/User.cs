using Microsoft.AspNetCore.Identity;

namespace system_university.Models
{
    public class User:IdentityUser
    {
        public string Role { get; set; } // Added Role property to store user role
    }
}
