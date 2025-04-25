using Microsoft.EntityFrameworkCore;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Course { get; set; }
        public DbSet<BasicCourse> BasicCourse { get; set; }
        public DbSet<BasicActivity> BasicActivity { get; set; }
        
        public DbSet<SystemUser> SystemUsers { get;  set; }
        public DbSet<Trainee> Trainee { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<BasicCourse>()
                .HasIndex(c => c.CourseName)
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasIndex(c => new { c.CourseName, c.CourseNo })
                .IsUnique();

             modelBuilder.Entity<BasicActivity>()
                .HasIndex(c => c.ActivityName )
                .IsUnique();

             modelBuilder.Entity<Trainee>()
                .HasIndex(c => new { c.PakNo, c.CourseId } )
                .IsUnique();
        }

        
    }
}