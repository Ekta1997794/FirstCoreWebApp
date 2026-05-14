using FirstCoreWebApp.Model;
using FirstCoreWebApp.Model.Course;
using FirstCoreWebApp.Model.Lesson;
using FirstCoreWebApp.Model.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FirstCoreWebApp
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        


        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoursePrerequisite>()
                .HasKey(cp => new
                {
                    cp.CourseId,
                    cp.PrerequisiteCourseId
                });

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.PrerequisiteCourse)
                .WithMany()
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            
        }

        
    }
}
