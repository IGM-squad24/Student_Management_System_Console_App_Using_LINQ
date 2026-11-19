namespace StudentManagementSystem.Models;

public class StudentSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public AcademicYear Year { get; set; }
    public int CourseCount { get; set; }
    public double Gpa { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DepartmentReportDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public double AverageGpa { get; set; }
    public double HighestGpa { get; set; }
    public double LowestGpa { get; set; }
    public string TopStudentName { get; set; } = string.Empty;
}

public class CourseAnalyticsDto
{
    public string CourseId { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int EnrolledStudents { get; set; }
    public double AverageScore { get; set; }
    public double HighestScore { get; set; }
    public double LowestScore { get; set; }
}

public class GradeDistributionDto
{
    public string GradeLetter { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
