namespace StudentManagementSystem.Models;

public class Course
{
    public string CourseId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int DepartmentId { get; set; }

    public Course() { }

    public Course(string courseId, string title, int credits, int departmentId)
    {
        CourseId = courseId;
        Title = title;
        Credits = credits;
        DepartmentId = departmentId;
    }
}
