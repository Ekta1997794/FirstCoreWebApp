using System.ComponentModel.DataAnnotations;

namespace FirstCoreWebApp.Model
{
    public class Module
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int CourseId { get; set; }

        public FirstCoreWebApp.Model.Course.Course Course { get; set; }
    }
}
