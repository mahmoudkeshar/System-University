using Microsoft.AspNetCore.Identity;

namespace system_university.Models
{
    public class Student : IdentityUser
    {
        public string FullName { get; set; }
    }
}
