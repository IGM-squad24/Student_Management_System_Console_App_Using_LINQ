using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services;

/// <summary>
/// Demonstrates comprehensive LINQ (Language Integrated Query) concepts in C#.
/// Covers Filtering, Projection, Sorting, Grouping, Aggregation, Joins, Set Operations, and Partitioning.
/// </summary>
public class StudentLinqService
{
    private readonly List<Student> _students;
    private readonly List<Department> _departments;
    private readonly List<Course> _courses;

    public StudentLinqService(List<Student> students, List<Department> departments, List<Course> courses)
    {
        _students = students;
        _departments = departments;
        _courses = courses;
    }

    #region 1. Projection & DTO Mapping (Select, SelectMany)

    /// <summary>
    /// LINQ: Select - Projects full student domain entities into lightweight summary DTOs.
    /// </summary>
    public List<StudentSummaryDto> GetStudentSummaries()
    {
        return _students
            .Select(s => new StudentSummaryDto
            {
                Id = s.Id,
                FullName = s.FullName,
                Department = s.DepartmentName,
                Year = s.Year,
                CourseCount = s.Enrollments.Count,
                Gpa = s.Gpa,
                Status = s.IsActive ? "Active" : "Inactive"
            })
            .ToList();
    }

    /// <summary>
    /// LINQ: SelectMany - Flattens nested collections (students -> enrollments) 
    /// paired with the parent student details.
    /// </summary>
    public IEnumerable<(Student Student, Enrollment Enrollment)> GetAllEnrollmentPairs()
    {
        return _students.SelectMany(
            student => student.Enrollments,
            (student, enrollment) => (Student: student, Enrollment: enrollment)
        );
    }

    #endregion

    #region 2. Filtering & Searching (Where, FirstOrDefault, SingleOrDefault)

    /// <summary>
    /// LINQ: Where - Dynamic search across multiple fields (ID, Name, Department, Year, GPA).
    /// </summary>
    public List<Student> SearchStudents(string? searchTerm = null, int? departmentId = null, 
                                        AcademicYear? year = null, double? minGpa = null, 
                                        double? maxGpa = null, bool? isActive = null)
    {
        var query = _students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.FullName.ToLowerInvariant().Contains(term) ||
                s.Email.ToLowerInvariant().Contains(term) ||
                s.Id.ToString().Contains(term));
        }

        if (departmentId.HasValue && departmentId.Value > 0)
        {
            query = query.Where(s => s.DepartmentId == departmentId.Value);
        }

        if (year.HasValue)
        {
            query = query.Where(s => s.Year == year.Value);
        }

        if (minGpa.HasValue)
        {
            query = query.Where(s => s.Gpa >= minGpa.Value);
        }

        if (maxGpa.HasValue)
        {
            query = query.Where(s => s.Gpa <= maxGpa.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        return query.ToList();
    }

    /// <summary>
    /// LINQ: SingleOrDefault - Safely finds a single unique student by ID.
    /// </summary>
    public Student? FindStudentById(int id)
    {
        return _students.SingleOrDefault(s => s.Id == id);
    }

    #endregion

    #region 3. Sorting & Ordering (OrderBy, OrderByDescending, ThenBy, ThenByDescending)

    /// <summary>
    /// LINQ: OrderBy / OrderByDescending + ThenBy - Flexible multi-level sorting.
    /// </summary>
    public List<Student> GetSortedStudents(string sortBy = "Name", bool ascending = true)
    {
        IOrderedEnumerable<Student> sorted = sortBy.ToLowerInvariant() switch
        {
            "id" => ascending ? _students.OrderBy(s => s.Id) : _students.OrderByDescending(s => s.Id),
            "gpa" => ascending 
                ? _students.OrderBy(s => s.Gpa).ThenBy(s => s.LastName) 
                : _students.OrderByDescending(s => s.Gpa).ThenBy(s => s.LastName),
            "age" => ascending 
                ? _students.OrderBy(s => s.Age).ThenBy(s => s.Gpa) 
                : _students.OrderByDescending(s => s.Age).ThenByDescending(s => s.Gpa),
            "department" => ascending 
                ? _students.OrderBy(s => s.DepartmentName).ThenByDescending(s => s.Gpa) 
                : _students.OrderByDescending(s => s.DepartmentName).ThenByDescending(s => s.Gpa),
            "year" => ascending 
                ? _students.OrderBy(s => s.Year).ThenByDescending(s => s.Gpa) 
                : _students.OrderByDescending(s => s.Year).ThenByDescending(s => s.Gpa),
            _ => ascending 
                ? _students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName) 
                : _students.OrderByDescending(s => s.LastName).ThenByDescending(s => s.FirstName)
        };

        return sorted.ToList();
    }

    #endregion

    #region 4. Partitioning & Pagination (Skip, Take, TakeWhile, SkipWhile)

    /// <summary>
    /// LINQ: Skip and Take - Standard pagination over collections.
    /// </summary>
    public (List<Student> PageItems, int TotalCount, int TotalPages) GetPagedStudents(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 5;

        var totalCount = _students.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = _students
            .OrderBy(s => s.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount, totalPages);
    }

    /// <summary>
    /// LINQ: OrderByDescending + Take - Extracts top N performing students by GPA.
    /// </summary>
    public List<Student> GetTopStudents(int count = 5)
    {
        return _students
            .OrderByDescending(s => s.Gpa)
            .ThenByDescending(s => s.AverageScore)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// LINQ: TakeWhile - Retrieves students sorted by GPA until GPA drops below a threshold.
    /// Demonstrates conditional slicing.
    /// </summary>
    public List<Student> GetStudentsAboveGpaThreshold(double threshold = 3.5)
    {
        return _students
            .OrderByDescending(s => s.Gpa)
            .TakeWhile(s => s.Gpa >= threshold)
            .ToList();
    }

    #endregion

    #region 5. Aggregations & Quantifiers (Count, Sum, Average, Min, Max, Any, All)

    /// <summary>
    /// LINQ: Aggregation functions Count, Average, Min, Max, Sum.
    /// </summary>
    public (int TotalStudents, double AverageGpa, double MaxGpa, double MinGpa, int TotalCreditsAwarded) GetOverallStatistics()
    {
        if (!_students.Any())
            return (0, 0.0, 0.0, 0.0, 0);

        var total = _students.Count;
        var avgGpa = Math.Round(_students.Average(s => s.Gpa), 2);
        var maxGpa = _students.Max(s => s.Gpa);
        var minGpa = _students.Min(s => s.Gpa);
        var totalCredits = _students.Sum(s => s.TotalCredits);

        return (total, avgGpa, maxGpa, minGpa, totalCredits);
    }

    /// <summary>
    /// LINQ: All - Students whose every single enrollment score is >= passingScore (e.g. 70).
    /// </summary>
    public List<Student> GetConsistentHighAchievers(double passingScore = 80.0)
    {
        return _students
            .Where(s => s.Enrollments.Any() && s.Enrollments.All(e => e.Score >= passingScore))
            .OrderByDescending(s => s.Gpa)
            .ToList();
    }

    /// <summary>
    /// LINQ: Any - Students who have at least one failing grade (Score < 60).
    /// </summary>
    public List<Student> GetAtRiskStudents(double failThreshold = 65.0)
    {
        return _students
            .Where(s => s.Enrollments.Any(e => e.Score < failThreshold) || s.Gpa < 2.0)
            .OrderBy(s => s.Gpa)
            .ToList();
    }

    #endregion

    #region 6. Grouping (GroupBy)

    /// <summary>
    /// LINQ: GroupBy - Groups students by department and computes comprehensive department-level metrics.
    /// </summary>
    public List<DepartmentReportDto> GetDepartmentReports()
    {
        return _students
            .GroupBy(s => new { s.DepartmentId, s.DepartmentName })
            .Select(g =>
            {
                var topStudent = g.OrderByDescending(s => s.Gpa).FirstOrDefault();
                return new DepartmentReportDto
                {
                    DepartmentId = g.Key.DepartmentId,
                    DepartmentName = g.Key.DepartmentName,
                    StudentCount = g.Count(),
                    AverageGpa = Math.Round(g.Average(s => s.Gpa), 2),
                    HighestGpa = g.Max(s => s.Gpa),
                    LowestGpa = g.Min(s => s.Gpa),
                    TopStudentName = topStudent != null ? $"{topStudent.FullName} ({topStudent.Gpa:F2})" : "N/A"
                };
            })
            .OrderByDescending(d => d.AverageGpa)
            .ToList();
    }

    /// <summary>
    /// LINQ: GroupBy - Groups students by Academic Year.
    /// </summary>
    public Dictionary<AcademicYear, List<Student>> GetStudentsByAcademicYear()
    {
        return _students
            .GroupBy(s => s.Year)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// LINQ: GroupBy + SelectMany - Analyzes all course enrollments to calculate grade letter frequencies.
    /// </summary>
    public List<GradeDistributionDto> GetGradeDistribution()
    {
        var allEnrollments = _students.SelectMany(s => s.Enrollments).ToList();
        var total = allEnrollments.Count;

        if (total == 0) return new List<GradeDistributionDto>();

        var validGrades = new[] { "A", "B", "C", "D", "F" };

        var grouped = allEnrollments
            .GroupBy(e => e.GradeLetter)
            .ToDictionary(g => g.Key, g => g.Count());

        return validGrades
            .Select(grade =>
            {
                var count = grouped.ContainsKey(grade) ? grouped[grade] : 0;
                var pct = Math.Round((double)count / total * 100, 1);
                return new GradeDistributionDto
                {
                    GradeLetter = grade,
                    Count = count,
                    Percentage = pct
                };
            })
            .ToList();
    }

    #endregion

    #region 7. Joins & Lookups (Join, GroupJoin, ToLookup, ToDictionary)

    /// <summary>
    /// LINQ: GroupJoin (Left Outer Join) - Joins Departments with Students,
    /// ensuring departments with 0 students are still included.
    /// </summary>
    public IEnumerable<(Department Dept, List<Student> Students)> GetDepartmentsWithStudents()
    {
        return _departments.GroupJoin(
            _students,
            dept => dept.Id,
            student => student.DepartmentId,
            (dept, studentGroup) => (Dept: dept, Students: studentGroup.ToList())
        );
    }

    /// <summary>
    /// LINQ: Join + SelectMany + GroupBy - Detailed course analytics joining Course definitions
    /// with student enrollments.
    /// </summary>
    public List<CourseAnalyticsDto> GetCourseAnalytics()
    {
        var allEnrollments = _students.SelectMany(s => s.Enrollments);

        // Group enrollments by CourseId
        var enrollmentGroups = allEnrollments.GroupBy(e => e.CourseId);

        // Inner Join between Courses catalog and aggregated enrollments
        var result = _courses.Join(
            enrollmentGroups,
            course => course.CourseId,
            group => group.Key,
            (course, group) => new CourseAnalyticsDto
            {
                CourseId = course.CourseId,
                CourseTitle = course.Title,
                EnrolledStudents = group.Count(),
                AverageScore = Math.Round(group.Average(e => e.Score), 1),
                HighestScore = group.Max(e => e.Score),
                LowestScore = group.Min(e => e.Score)
            }
        )
        .OrderByDescending(c => c.EnrolledStudents)
        .ThenByDescending(c => c.AverageScore)
        .ToList();

        return result;
    }

    /// <summary>
    /// LINQ: ToLookup - Creates an efficient 1-to-many lookup indexed by Department Id.
    /// </summary>
    public ILookup<int, Student> GetStudentsByDepartmentLookup()
    {
        return _students.ToLookup(s => s.DepartmentId);
    }

    #endregion

    #region 8. Set Operations (Distinct, Intersect, Union, Except)

    /// <summary>
    /// LINQ: Distinct - Extracts all distinct course IDs currently taken by active students.
    /// </summary>
    public List<string> GetDistinctEnrolledCourses()
    {
        return _students
            .Where(s => s.IsActive)
            .SelectMany(s => s.Enrollments)
            .Select(e => e.CourseId)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    /// <summary>
    /// LINQ: Intersect - Students enrolled in BOTH course 1 and course 2.
    /// </summary>
    public List<Student> GetStudentsInBothCourses(string courseId1, string courseId2)
    {
        var studentsInCourse1 = _students
            .Where(s => s.Enrollments.Any(e => e.CourseId.Equals(courseId1, StringComparison.OrdinalIgnoreCase)));

        var studentsInCourse2 = _students
            .Where(s => s.Enrollments.Any(e => e.CourseId.Equals(courseId2, StringComparison.OrdinalIgnoreCase)));

        return studentsInCourse1.Intersect(studentsInCourse2).ToList();
    }

    /// <summary>
    /// LINQ: Union - All unique students enrolled in EITHER course 1 or course 2.
    /// </summary>
    public List<Student> GetStudentsInEitherCourse(string courseId1, string courseId2)
    {
        var studentsInCourse1 = _students
            .Where(s => s.Enrollments.Any(e => e.CourseId.Equals(courseId1, StringComparison.OrdinalIgnoreCase)));

        var studentsInCourse2 = _students
            .Where(s => s.Enrollments.Any(e => e.CourseId.Equals(courseId2, StringComparison.OrdinalIgnoreCase)));

        return studentsInCourse1.Union(studentsInCourse2).ToList();
    }

    /// <summary>
    /// LINQ: Except - Students enrolled in course 1 but NOT course 2.
    /// </summary>
    public List<Student> GetStudentsInCourse1NotCourse2(string courseId1, string courseId2)
    {
        var studentsInCourse1 = _students
            .Where(s => s.Enrollments.Any(e => e.CourseId.Equals(courseId1, StringComparison.OrdinalIgnoreCase)));

        var studentsInCourse2 = _students
            .Where(s => s.Enrollments.Any(e => e.CourseId.Equals(courseId2, StringComparison.OrdinalIgnoreCase)));

        return studentsInCourse1.Except(studentsInCourse2).ToList();
    }

    #endregion
}
