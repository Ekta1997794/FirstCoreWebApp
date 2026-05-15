using System.ComponentModel.DataAnnotations;

namespace FirstCoreWebApp.Model.Material
{
    public class CreateLessonDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }


        public List<int>? CourseMaterialIds { get; set; }
    }
}
