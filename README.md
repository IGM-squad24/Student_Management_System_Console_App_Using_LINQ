# Student Management System (Powered by LINQ)

A modern, full-featured **Student Management System** console application written in C# (.NET 9) designed specifically to demonstrate and master **Language Integrated Query (LINQ)** concepts.

---

## 🌟 Key Features

1. **Complete Student Records Lifecycle (CRUD)**
   - Add new students with validation and auto-calculated IDs (`.Max() + 1`).
   - Browse student directory with interactive pagination (`.Skip()` and `.Take()`).
   - View detailed transcripts with weighted GPA calculations.
   - Update student details and manage course enrollments / scores.
   - Delete records with confirmation.
   - Persistent storage via `students.json` with fallback demonstration seed data.

2. **LINQ Search & Multi-Criteria Filtering**
   - Search by keyword across ID, full name, or email (`.Where()` + `.Contains()`).
   - Filter by department, academic year (Freshman to Senior), and GPA cutoffs.
   - Dynamic chaining of IQueryable predicates.

3. **Dynamic Sorting Playground**
   - Multi-level ordering (`.OrderBy()`, `.OrderByDescending()`, `.ThenBy()`, `.ThenByDescending()`).
   - Sort by Name, GPA, Age, Department, or Year with ascending/descending toggle.

4. **LINQ Analytics & Reports Dashboard**
   - **Department Analytics**: Average, Min, and Max GPA per department with top student highlighted (`.GroupBy()`, `.Average()`, `.Min()`, `.Max()`).
   - **Top N Performers**: Dynamic leaderboard (`.OrderByDescending().Take(n)`).
   - **Honor Roll Cutoff**: Conditional slicing with `.TakeWhile()`.
   - **Academic Year Distribution**: Grouping students by study year (`.GroupBy().ToDictionary()`).
   - **Grade Distribution Chart**: ASCII frequency distribution of course grades (`.SelectMany().GroupBy()`).
   - **Course Analytics**: Inner join between catalog courses and enrollments (`.Join().GroupBy()`).
   - **Achievers vs. At-Risk Students**: High-achiever inspection (`.All()`) and early intervention alerts (`.Any()`).
   - **Overall University Metrics**: Total students, university-wide average GPA, and total credits (`.Sum()`, `.Average()`, `.Count()`).

5. **Mathematical Set Operations**
   - Compare any two courses side-by-side using:
     - `Intersect()`: Students enrolled in *both* courses.
     - `Except()`: Students enrolled in Course A but *not* Course B.
     - `Union()`: All unique students enrolled in *either* course.
     - `Distinct()`: Unique course catalog references.

---

## 📚 LINQ Concepts Mapped to Code

| LINQ Category | Methods Used | Practical Application in System |
|---|---|---|
| **Projection** | `.Select()`, `.SelectMany()` | Transforming `Student` to `StudentSummaryDto`; flattening nested student course enrollments for university-wide grade distributions. |
| **Filtering** | `.Where()`, `.SingleOrDefault()`, `.FirstOrDefault()` | Multi-criteria search (name, email, department, year, GPA); finding unique students by ID. |
| **Ordering** | `.OrderBy()`, `.OrderByDescending()`, `.ThenBy()` | Sorting students by primary fields (GPA, Age, Department) with secondary ties broken by Name. |
| **Partitioning** | `.Skip()`, `.Take()`, `.TakeWhile()` | Paginating directory lists (`Skip((p-1)*size).Take(size)`); slicing top N students and honor roll cutoffs. |
| **Aggregation** | `.Count()`, `.Sum()`, `.Average()`, `.Min()`, `.Max()` | Calculating cumulative GPA, average course scores, total credits earned, and department stats. |
| **Quantifiers** | `.Any()`, `.All()`, `.Contains()` | Detecting at-risk students (`Any(score < 65)`), honor students (`All(score >= 80)`), and substring searches. |
| **Grouping** | `.GroupBy()`, `.ToDictionary()`, `.ToLookup()` | Grouping students by Department and Academic Year; letter grade frequency tables; 1-to-many lookup tables. |
| **Joins** | `.Join()`, `.GroupJoin()` | Inner joining course catalog with enrollments; outer group join of departments with students (including empty departments). |
| **Set Operations** | `.Distinct()`, `.Intersect()`, `.Union()`, `.Except()` | Comparing course cohorts (students in both classes, only in one class, or either class). |

---

## 📂 Project Architecture

```
Student_Management_System/
│
├── Models/
│   ├── Enums.cs              # Gender, AcademicYear enums
│   ├── Department.cs         # Department entity
│   ├── Course.cs             # Course definition entity
│   ├── Enrollment.cs         # Student course enrollment & grade conversion
│   ├── Student.cs            # Student domain entity with LINQ computed properties
│   └── Dtos.cs               # Projections for LINQ summaries & reports
│
├── Data/
│   ├── SeedData.cs           # 12+ preloaded students with realistic enrollments
│   └── StudentRepository.cs  # CRUD and JSON file persistence (students.json)
│
├── Services/
│   └── StudentLinqService.cs # Pure LINQ query implementations with comments
│
├── UI/
│   ├── ConsoleHelper.cs      # ANSI color formatting, tables, prompts, and LINQ tags
│   └── MenuHandler.cs        # Interactive terminal navigation & workflows
│
├── Tests/
│   └── StudentLinqServiceTests.cs # 11 xUnit tests validating all LINQ queries
│
├── Program.cs                # Application entry point
├── StudentManagementSystem.csproj
└── StudentManagementSystem.sln
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later installed.

### Run the Application
From the workspace root directory:
```bash
dotnet run
```

### Run Unit Tests
To verify all LINQ queries and operations:
```bash
dotnet test
```
All 11 unit tests should pass with 0 warnings and 0 errors.

---

## 💡 Code Highlights

### 1. Department Summary with `.GroupBy()` and Aggregations
```csharp
public List<DepartmentReportDto> GetDepartmentReports()
{
    return _students
        .GroupBy(s => new { s.DepartmentId, s.DepartmentName })
        .Select(g => new DepartmentReportDto
        {
            DepartmentId = g.Key.DepartmentId,
            DepartmentName = g.Key.DepartmentName,
            StudentCount = g.Count(),
            AverageGpa = Math.Round(g.Average(s => s.Gpa), 2),
            HighestGpa = g.Max(s => s.Gpa),
            LowestGpa = g.Min(s => s.Gpa),
            TopStudentName = g.OrderByDescending(s => s.Gpa).First().FullName
        })
        .OrderByDescending(d => d.AverageGpa)
        .ToList();
}
```

### 2. Flattening & Grade Distribution with `.SelectMany()` & `.GroupBy()`
```csharp
var allEnrollments = _students.SelectMany(s => s.Enrollments).ToList();
var grouped = allEnrollments
    .GroupBy(e => e.GradeLetter)
    .ToDictionary(g => g.Key, g => g.Count());
```

### 3. Comparing Cohorts with `.Intersect()` and `.Except()`
```csharp
var inCourse1 = _students.Where(s => s.Enrollments.Any(e => e.CourseId == "CS101"));
var inCourse2 = _students.Where(s => s.Enrollments.Any(e => e.CourseId == "MATH101"));

var bothCourses = inCourse1.Intersect(inCourse2).ToList();
var course1Only = inCourse1.Except(inCourse2).ToList();
var eitherCourse = inCourse1.Union(inCourse2).ToList();
```
