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

        public DbSet<ActivityPlanner> ActivityPlanner { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Evaluation> Evaluation { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormColumn> FormColumns { get; set; }
        public DbSet<FormData> FormData { get; set; }


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
                
             modelBuilder.Entity<ActivityPlanner>()
                .HasIndex(c => new { c.ActivityId, c.CourseId, c.Date } )
                .IsUnique();

             modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.ActivityId, a.CourseId, a.TraineeId })
                .IsUnique();

             modelBuilder.Entity<Evaluation>()
                .HasIndex(e => new { e.ActivityId, e.CourseId, e.TraineeId })
                .IsUnique();
            
            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormColumns)
                .WithOne(fc => fc.Form)
                .HasForeignKey(fc => fc.FormId);

            modelBuilder.Entity<Form>()
                .HasMany(f => f.FormData)
                .WithOne(fd => fd.Form)
                .HasForeignKey(fd => fd.FormId);

            modelBuilder.Entity<FormColumn>()
                .HasMany(fc => fc.FormData)
                .WithOne(fd => fd.FormColumn)
                .HasForeignKey(fd => fd.ColumnId);

        }

        
    }
}