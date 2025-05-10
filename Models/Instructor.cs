namespace system_university.Models
{
    public class Instructor : User
    {
        public ICollection<Subject> Subjects { get; set; } // Added Subjects property to establish relationship with Subject
    }
}
