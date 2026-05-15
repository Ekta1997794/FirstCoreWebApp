using System.Collections.Generic;
using FirstCoreWebApp.Model.Course;
using FirstCoreWebApp.Model.Material;

namespace FirstCoreWebApp.Model.Lesson
{
    public class Lesson
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        // FK
        public int CourseId { get; set; }

        // Navigation
        public FirstCoreWebApp.Model.Course.Course Course { get; set; }

        // Many-to-many with CourseMaterial
        public ICollection<LessonMaterial> LessonMaterials { get; set; }
            = new List<LessonMaterial>();
    }
}