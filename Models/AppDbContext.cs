using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace system_university.Models
{
    public class AppDbContext : IdentityDbContext<User>  
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Schedule>().HasData(
                new Schedule {Id = 1,Level = 1,StudentSchedule = "/schedules/level1.pdf"},
                new Schedule{Id = 2,Level = 2,StudentSchedule = "/schedules/level2.pdf"},
                new Schedule{Id = 3,Level = 3,StudentSchedule = "/schedules/level3.pdf"},
                new Schedule{Id = 4,Level = 4,StudentSchedule = "/schedules/level4.pdf"}
            );

            modelBuilder.Entity<Student>()
                        .HasIndex(s => s.StudentCode)
                        .IsUnique();

            modelBuilder.Entity<Instructor>()
                        .HasMany(i => i.Subjects)
                        .WithMany(s => s.Instructors)
                        .UsingEntity(j => j.ToTable("InstructorSubjects"));

            modelBuilder.Entity<Subject>().HasData(
                        new Subject { Id = 1, Name = "Programming" },
                        new Subject { Id = 2, Name = "Mathematics" },
                        new Subject { Id = 3, Name = "Artificial Intelligence" },
                        new Subject { Id = 4, Name = "Databases" },
                        new Subject { Id = 5, Name = "Data Structures" },
                        new Subject { Id = 6, Name = "Operating Systems" },
                        new Subject { Id = 7, Name = "Computer Networks" });
        }


        public DbSet<Student> Students { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        public DbSet<DegreeOfQuizes> degreeOfQuizes { get; set; }

    }
}