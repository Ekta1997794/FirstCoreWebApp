using FirstCoreWebApp.Model.Lesson;

namespace FirstCoreWebApp.Model.Material
{
    public class LessonMaterial
    {
        public int Id { get; set; }

        public int LessonId { get; set; }
        public FirstCoreWebApp.Model.Lesson.Lesson Lesson { get; set; }

        public int CourseMaterialId { get; set; }
        public CourseMaterial CourseMaterial { get; set; }
    }
}
