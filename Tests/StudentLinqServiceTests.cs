using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using Xunit;

namespace StudentManagementSystem.Tests;

public class StudentLinqServiceTests
{
    private readonly StudentLinqService _service;
    private readonly List<Student> _students;
    private readonly List<Department> _departments;
    private readonly List<Course> _courses;

    public StudentLinqServiceTests()
    {
        _students = SeedData.GetStudents();
        _departments = SeedData.GetDepartments();
        _courses = SeedData.GetCourses();
        _service = new StudentLinqService(_students, _departments, _courses);
    }

    [Fact]
    public void Select_ProjectsToSummaryDto()
    {
        var summaries = _service.GetStudentSummaries();

        Assert.Equal(_students.Count, summaries.Count);
        Assert.All(summaries, s =>
        {
            Assert.True(s.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(s.FullName));
            Assert.False(string.IsNullOrWhiteSpace(s.Department));
        });
    }

    [Fact]
    public void Where_FiltersByKeywordAndDepartment()
    {
        var csStudents = _service.SearchStudents(departmentId: 1); // CS has ID 1
        Assert.NotEmpty(csStudents);
        Assert.All(csStudents, s => Assert.Equal(1, s.DepartmentId));

        var aliceResult = _service.SearchStudents(searchTerm: "Alice");
        Assert.Single(aliceResult);
        Assert.Equal("Alice Johnson", aliceResult[0].FullName);
    }

    [Fact]
    public void OrderBy_SortsByGpaDescending()
    {
        var sorted = _service.GetSortedStudents("gpa", ascending: false);

        for (int i = 0; i < sorted.Count - 1; i++)
        {
            Assert.True(sorted[i].Gpa >= sorted[i + 1].Gpa);
        }
    }

    [Fact]
    public void SkipAndTake_PaginatesCorrectly()
    {
        var pageSize = 4;
        var (page1, totalCount, totalPages) = _service.GetPagedStudents(1, pageSize);
        var (page2, _, _) = _service.GetPagedStudents(2, pageSize);

        Assert.Equal(pageSize, page1.Count);
        Assert.Equal(pageSize, page2.Count);
        Assert.Equal(_students.Count, totalCount);

        // Page 1 and Page 2 must not have overlapping IDs
        var page1Ids = page1.Select(s => s.Id);
        var page2Ids = page2.Select(s => s.Id);
        Assert.Empty(page1Ids.Intersect(page2Ids));
    }

    [Fact]
    public void TakeWhile_RetrievesOnlyStudentsAboveCutoff()
    {
        double cutoff = 3.5;
        var honors = _service.GetStudentsAboveGpaThreshold(cutoff);

        Assert.NotEmpty(honors);
        Assert.All(honors, s => Assert.True(s.Gpa >= cutoff));
    }

    [Fact]
    public void Aggregates_CalculateAccurateStatistics()
    {
        var (total, avgGpa, maxGpa, minGpa, totalCredits) = _service.GetOverallStatistics();

        Assert.Equal(_students.Count, total);
        Assert.True(avgGpa > 0 && avgGpa <= 4.0);
        Assert.True(maxGpa >= minGpa);
        Assert.True(totalCredits > 0);
    }

    [Fact]
    public void AllAndAny_CorrectlyIdentifiesAchieversAndAtRisk()
    {
        var achievers = _service.GetConsistentHighAchievers(80.0);
        Assert.All(achievers, s => Assert.True(s.Enrollments.All(e => e.Score >= 80.0)));

        var atRisk = _service.GetAtRiskStudents(65.0);
        Assert.All(atRisk, s => Assert.True(s.Enrollments.Any(e => e.Score < 65.0) || s.Gpa < 2.0));
    }

    [Fact]
    public void GroupBy_GeneratesDepartmentReports()
    {
        var reports = _service.GetDepartmentReports();

        Assert.NotEmpty(reports);
        var totalInReports = reports.Sum(r => r.StudentCount);
        Assert.Equal(_students.Count, totalInReports);
    }

    [Fact]
    public void SelectMany_And_GroupBy_GeneratesGradeDistribution()
    {
        var distribution = _service.GetGradeDistribution();

        Assert.NotEmpty(distribution);
        var totalGrades = distribution.Sum(g => g.Count);
        var expectedTotalGrades = _students.SelectMany(s => s.Enrollments).Count();
        Assert.Equal(expectedTotalGrades, totalGrades);
    }

    [Fact]
    public void Join_CalculatesCourseAnalytics()
    {
        var courseAnalytics = _service.GetCourseAnalytics();

        Assert.NotEmpty(courseAnalytics);
        Assert.All(courseAnalytics, ca =>
        {
            Assert.False(string.IsNullOrWhiteSpace(ca.CourseId));
            Assert.True(ca.EnrolledStudents > 0);
            Assert.True(ca.AverageScore >= 0 && ca.AverageScore <= 100);
        });
    }

    [Fact]
    public void SetOperations_Intersect_Union_Except()
    {
        var c1 = "CS101";
        var c2 = "MATH101";

        var intersect = _service.GetStudentsInBothCourses(c1, c2);
        var union = _service.GetStudentsInEitherCourse(c1, c2);
        var except = _service.GetStudentsInCourse1NotCourse2(c1, c2);

        // Every student in intersect must be enrolled in both
        Assert.All(intersect, s =>
        {
            Assert.Contains(s.Enrollments, e => e.CourseId == c1);
            Assert.Contains(s.Enrollments, e => e.CourseId == c2);
        });

        // Students in except must be in c1 and NOT in c2
        Assert.All(except, s =>
        {
            Assert.Contains(s.Enrollments, e => e.CourseId == c1);
            Assert.DoesNotContain(s.Enrollments, e => e.CourseId == c2);
        });

        // Intersect and Except are disjoint subsets of Union
        var intersectIds = intersect.Select(s => s.Id).ToHashSet();
        var exceptIds = except.Select(s => s.Id).ToHashSet();
        Assert.Empty(intersectIds.Intersect(exceptIds));
    }
}
