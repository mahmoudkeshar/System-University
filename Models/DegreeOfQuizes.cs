using System.ComponentModel.DataAnnotations;

namespace system_university.Models
{
    public class DegreeOfQuizes
    {
        [Key]
        public int Id { get; set; }
        public int StudentCode { get; set; }
        public string StudentName { get; set; }
        public int QuizCode { get; set; }
        public string Degree { get; set; }
    }
}
