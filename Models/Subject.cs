namespace system_university.Models
{
    public class Subject
    {
        public int Id { get; set; } // Adding the missing 'Id' property  
        public string Name { get; set; }
        public ICollection<Instructor> Instructors { get; set; }
    }
}
