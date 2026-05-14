
using FirstCoreWebApp.Model.Course;
using System.ComponentModel.DataAnnotations;

namespace FirstCoreWebApp.Model
{
    public class CreateCourseDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public CourseStatus? Status { get; set; }

        public List<int>? PrerequisiteCourseIds { get; set; }
    }
}

