using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace system_university.Models
{
    public class AppDbContext : IdentityDbContext  // Fix: Inherit from DbContext  
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
        }


        public DbSet<Student> Students { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
    }
}