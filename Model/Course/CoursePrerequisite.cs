namespace FirstCoreWebApp.Model.Course
{
    public class CoursePrerequisite
    {
        public int CourseId { get; set; }

        public Course Course { get; set; }

        public int PrerequisiteCourseId { get; set; }

        public Course PrerequisiteCourse { get; set; }
    }
    // Course and PrerequisiteCourse are the navigation fields which handling the EF core relation and represents the course
    //and PrerequisiteCourse objects
}
