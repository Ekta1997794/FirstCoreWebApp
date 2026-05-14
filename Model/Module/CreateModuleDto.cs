using System.ComponentModel.DataAnnotations;

namespace FirstCoreWebApp.Model
{
    public class CreateModuleDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
}
