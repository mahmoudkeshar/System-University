namespace system_university.Models
{
    public class RegisterInsDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<string> Subjects { get; set; } // Added Subjects property to establish relationship with Subject
    }
}
