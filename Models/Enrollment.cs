namespace StudentManagementSystem.Models;

public class Enrollment
{
    public string CourseId { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public double Score { get; set; } // 0 - 100
    public int Credits { get; set; } = 3;

    public string GradeLetter => Score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };

    public double GradePoint => Score switch
    {
        >= 90 => 4.0,
        >= 80 => 3.0,
        >= 70 => 2.0,
        >= 60 => 1.0,
        _ => 0.0
    };

    public Enrollment() { }

    public Enrollment(string courseId, string courseName, double score, int credits = 3)
    {
        CourseId = courseId;
        CourseName = courseName;
        Score = score;
        Credits = credits;
    }
}
