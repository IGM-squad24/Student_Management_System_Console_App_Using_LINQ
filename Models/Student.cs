using System.Text.Json.Serialization;

namespace StudentManagementSystem.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public AcademicYear Year { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Enrollment> Enrollments { get; set; } = new();

    // Calculated using LINQ concepts
    [JsonIgnore]
    public double Gpa
    {
        get
        {
            if (Enrollments == null || Enrollments.Count == 0) return 0.0;
            var totalCredits = Enrollments.Sum(e => e.Credits);
            if (totalCredits == 0) return 0.0;
            var weightedPoints = Enrollments.Sum(e => e.GradePoint * e.Credits);
            return Math.Round(weightedPoints / totalCredits, 2);
        }
    }

    [JsonIgnore]
    public double AverageScore =>
        Enrollments != null && Enrollments.Any()
            ? Math.Round(Enrollments.Average(e => e.Score), 1)
            : 0.0;

    [JsonIgnore]
    public int TotalCredits =>
        Enrollments?.Sum(e => e.Credits) ?? 0;

    [JsonIgnore]
    public string PerformanceCategory => Gpa switch
    {
        >= 3.8 => "High Honors",
        >= 3.5 => "Honors",
        >= 3.0 => "Good Standing",
        >= 2.0 => "Satisfactory",
        _ => "Academic Probation"
    };

    public Student() { }

    public Student(int id, string firstName, string lastName, string email, int age, 
                   Gender gender, int departmentId, string departmentName, AcademicYear year, 
                   DateTime enrollmentDate, bool isActive = true)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Age = age;
        Gender = gender;
        DepartmentId = departmentId;
        DepartmentName = departmentName;
        Year = year;
        EnrollmentDate = enrollmentDate;
        IsActive = isActive;
    }
}
