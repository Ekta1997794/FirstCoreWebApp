using FirstCoreWebApp.Model;
using System.ComponentModel.DataAnnotations;

namespace FirstCoreWebApp
{
    public class UpdateCourseDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        
    }
}
