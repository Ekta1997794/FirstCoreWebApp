using System.ComponentModel.DataAnnotations;
using FirstCoreWebApp.Model.Material;
using FirstCoreWebApp.Model.Lesson;

namespace FirstCoreWebApp.Model.Course
{
    public enum CourseStatus
    {
        Draft,
        Published,
        Archived
    }
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public CourseStatus? Status { get; set; }

        public ICollection<CoursePrerequisite>? Prerequisites { get; set; }

        public string? Category { get; set; }
        public int? InstructorId { get; set; }
        public ICollection<Module>? Modules { get; set; }
        public ICollection<FirstCoreWebApp.Model.Lesson.Lesson>? Lessons { get; set; }

        public ICollection<CourseMaterial>? CourseMaterials { get; set; }
    }
}
