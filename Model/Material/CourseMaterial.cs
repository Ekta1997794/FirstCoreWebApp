using FirstCoreWebApp.Model;

namespace FirstCoreWebApp.Model.Material
{
    public class CourseMaterial
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string ContentType { get; set; }

        public FirstCoreWebApp.Model.Course.Course Course { get; set; }
    }
}