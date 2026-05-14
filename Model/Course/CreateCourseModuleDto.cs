using System.ComponentModel.DataAnnotations;

namespace FirstCoreWebApp.Model
{
    public class CreateCourseModuleDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }
    }
}
