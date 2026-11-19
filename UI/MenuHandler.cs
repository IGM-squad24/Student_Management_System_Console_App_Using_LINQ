using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.UI;

public class MenuHandler
{
    private readonly StudentRepository _repository;
    private StudentLinqService _linqService;

    public MenuHandler(StudentRepository repository)
    {
        _repository = repository;
        _linqService = new StudentLinqService(_repository.GetAll(), _repository.GetDepartments(), _repository.GetCourses());
    }

    private void RefreshLinqService()
    {
        _linqService = new StudentLinqService(_repository.GetAll(), _repository.GetDepartments(), _repository.GetCourses());
    }

    public void Run()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("STUDENT MANAGEMENT SYSTEM  (POWERED BY LINQ)");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  1. Student Records & CRUD Operations");
            Console.WriteLine("  2. LINQ Search & Multi-criteria Filtering");
            Console.WriteLine("  3. LINQ Dynamic Sorting Playground");
            Console.WriteLine("  4. LINQ Analytics & Reports Dashboard");
            Console.WriteLine("  5. Course Comparison & Set Operations (Intersect / Union / Except)");
            Console.WriteLine("  6. System Data Management (Save / Reset Seed Data)");
            Console.WriteLine("  0. Exit Application");
            Console.ResetColor();

            Console.WriteLine(new string('-', 78));
            var choice = ConsoleHelper.ReadInt("Select an option (0-6)", 0, 6);

            switch (choice)
            {
                case 1:
                    ManageStudentRecordsMenu();
                    break;
                case 2:
                    SearchAndFilterMenu();
                    break;
                case 3:
                    SortingPlaygroundMenu();
                    break;
                case 4:
                    AnalyticsDashboardMenu();
                    break;
                case 5:
                    SetOperationsMenu();
                    break;
                case 6:
                    DataManagementMenu();
                    break;
                case 0:
                    exit = true;
                    Console.Clear();
                    ConsoleHelper.PrintHeader("Thank You for Using Student Management System");
                    Console.WriteLine("Goodbye!\n");
                    break;
            }
        }
    }

    #region 1. Student Records & CRUD

    private void ManageStudentRecordsMenu()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("STUDENT RECORDS MANAGEMENT");
            Console.WriteLine("  1. View All Students (Paged with LINQ Skip/Take)");
            Console.WriteLine("  2. View Full Student Profile & Grades");
            Console.WriteLine("  3. Add New Student");
            Console.WriteLine("  4. Update Existing Student");
            Console.WriteLine("  5. Enroll Student in Course / Add Grade");
            Console.WriteLine("  6. Delete Student");
            Console.WriteLine("  0. Back to Main Menu");
            Console.WriteLine(new string('-', 78));

            var choice = ConsoleHelper.ReadInt("Select an option (0-6)", 0, 6);
            switch (choice)
            {
                case 1:
                    ViewPagedStudents();
                    break;
                case 2:
                    ViewStudentProfile();
                    break;
                case 3:
                    AddNewStudent();
                    break;
                case 4:
                    UpdateStudent();
                    break;
                case 5:
                    EnrollStudentInCourse();
                    break;
                case 6:
                    DeleteStudent();
                    break;
                case 0:
                    back = true;
                    break;
            }
        }
    }

    private void ViewPagedStudents()
    {
        int page = 1;
        int pageSize = 5;

        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader($"STUDENT DIRECTORY - PAGE {page}");
            ConsoleHelper.PrintLinqTag("Skip() & Take()", $"Paginates data: .Skip(({page}-1) * {pageSize}).Take({pageSize})");

            var (items, totalCount, totalPages) = _linqService.GetPagedStudents(page, pageSize);
            ConsoleHelper.PrintStudentTable(items);

            Console.WriteLine($"\nPage {page} of {Math.Max(1, totalPages)} (Total Students: {totalCount})");
            Console.WriteLine("[N] Next Page | [P] Previous Page | [S] Change Page Size | [B] Back");
            var key = ConsoleHelper.ReadString("Action", false).ToUpperInvariant();

            if (key == "N" && page < totalPages) page++;
            else if (key == "P" && page > 1) page--;
            else if (key == "S")
            {
                pageSize = ConsoleHelper.ReadInt("Enter items per page (1-20)", 1, 20, 5);
                page = 1;
            }
            else if (key == "B" || string.IsNullOrEmpty(key)) break;
        }
    }

    private void ViewStudentProfile()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("STUDENT PROFILE & TRANSCRIPT");
        ConsoleHelper.PrintLinqTag("SingleOrDefault()", "Retrieves unique student record matching Id");

        var id = ConsoleHelper.ReadInt("Enter Student ID to inspect");
        var student = _linqService.FindStudentById(id);

        if (student == null)
        {
            ConsoleHelper.PrintError($"Student with ID {id} not found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n[STUDENT ID]     : {student.Id}");
        Console.WriteLine($"[NAME]           : {student.FullName}");
        Console.WriteLine($"[EMAIL]          : {student.Email}");
        Console.WriteLine($"[AGE / GENDER]   : {student.Age} years old / {student.Gender}");
        Console.WriteLine($"[DEPARTMENT]     : {student.DepartmentName} (ID: {student.DepartmentId})");
        Console.WriteLine($"[ACADEMIC YEAR]  : {student.Year}");
        Console.WriteLine($"[ENROLLED DATE]  : {student.EnrollmentDate:yyyy-MM-dd}");
        Console.WriteLine($"[STATUS]         : {(student.IsActive ? "Active" : "Inactive")}");
        Console.WriteLine($"[CUMULATIVE GPA] : {student.Gpa:F2} / 4.00 ({student.PerformanceCategory})");
        Console.WriteLine($"[AVERAGE SCORE]  : {student.AverageScore}%");
        Console.WriteLine($"[TOTAL CREDITS]  : {student.TotalCredits}");
        Console.ResetColor();

        ConsoleHelper.PrintSubHeader("ENROLLED COURSES & GRADES (LINQ Projection)");
        if (!student.Enrollments.Any())
        {
            ConsoleHelper.PrintWarning("No course enrollments on record.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("+" + new string('-', 10) + "+" + new string('-', 35) + "+" + new string('-', 9) + "+" + new string('-', 7) + "+" + new string('-', 8) + "+");
            Console.WriteLine($"| {"Code",-8} | {"Course Title",-33} | {"Credits",-7} | {"Score",-5} | {"Grade",-6} |");
            Console.WriteLine("+" + new string('-', 10) + "+" + new string('-', 35) + "+" + new string('-', 9) + "+" + new string('-', 7) + "+" + new string('-', 8) + "+");
            Console.ResetColor();

            foreach (var e in student.Enrollments)
            {
                var gradeColor = e.GradeLetter switch
                {
                    "A" => ConsoleColor.Green,
                    "B" => ConsoleColor.Cyan,
                    "C" => ConsoleColor.Yellow,
                    _ => ConsoleColor.Red
                };

                Console.Write($"| {e.CourseId,-8} | {e.CourseName,-33} | {e.Credits,-7} | {e.Score,5:F1} | ");
                Console.ForegroundColor = gradeColor;
                Console.Write($"{e.GradeLetter,2} ({e.GradePoint:F1})");
                Console.ResetColor();
                Console.WriteLine(" |");
            }

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("+" + new string('-', 10) + "+" + new string('-', 35) + "+" + new string('-', 9) + "+" + new string('-', 7) + "+" + new string('-', 8) + "+");
            Console.ResetColor();
        }

        ConsoleHelper.Pause();
    }

    private void AddNewStudent()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("ADD NEW STUDENT RECORD");
        ConsoleHelper.PrintLinqTag("Max()", "Generates auto-increment ID using .Max(s => s.Id) + 1");

        var nextId = _repository.GetNextId();
        var id = ConsoleHelper.ReadInt("Student ID", 1, 999999, nextId);

        if (_repository.GetById(id) != null)
        {
            ConsoleHelper.PrintError($"Student with ID {id} already exists.");
            ConsoleHelper.Pause();
            return;
        }

        var firstName = ConsoleHelper.ReadString("First Name");
        var lastName = ConsoleHelper.ReadString("Last Name");
        var email = ConsoleHelper.ReadString("Email address");
        var age = ConsoleHelper.ReadInt("Age", 16, 99, 20);

        Console.WriteLine("\nSelect Gender: [0] Male  [1] Female  [2] Other");
        var genderChoice = ConsoleHelper.ReadInt("Gender", 0, 2, 0);
        var gender = (Gender)genderChoice;

        Console.WriteLine("\nAvailable Departments:");
        var depts = _repository.GetDepartments();
        foreach (var d in depts)
        {
            Console.WriteLine($"  [{d.Id}] {d.Name} ({d.Code})");
        }
        var deptId = ConsoleHelper.ReadInt("Select Department ID", 1, depts.Count, 1);
        var selectedDept = depts.FirstOrDefault(d => d.Id == deptId);
        var deptName = selectedDept?.Name ?? "Unknown";

        Console.WriteLine("\nSelect Academic Year: [1] Freshman  [2] Sophomore  [3] Junior  [4] Senior");
        var yearChoice = ConsoleHelper.ReadInt("Academic Year", 1, 4, 1);
        var year = (AcademicYear)yearChoice;

        var student = new Student(id, firstName, lastName, email, age, gender, deptId, deptName, year, DateTime.Now);

        if (_repository.Add(student))
        {
            RefreshLinqService();
            ConsoleHelper.PrintSuccess($"Student '{student.FullName}' (ID: {student.Id}) added successfully!");
        }
        else
        {
            ConsoleHelper.PrintError("Failed to add student.");
        }

        ConsoleHelper.Pause();
    }

    private void UpdateStudent()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("UPDATE STUDENT DETAILS");
        ConsoleHelper.PrintLinqTag("FirstOrDefault()", "Locates target record by Id");

        var id = ConsoleHelper.ReadInt("Enter Student ID to update");
        var student = _repository.GetById(id);

        if (student == null)
        {
            ConsoleHelper.PrintError($"Student with ID {id} not found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine($"Updating details for: {student.FullName} (Press Enter to keep current value)");

        student.FirstName = ConsoleHelper.ReadString("First Name", false, student.FirstName);
        student.LastName = ConsoleHelper.ReadString("Last Name", false, student.LastName);
        student.Email = ConsoleHelper.ReadString("Email", false, student.Email);
        student.Age = ConsoleHelper.ReadInt("Age", 16, 99, student.Age);

        Console.WriteLine("\nAvailable Departments:");
        var depts = _repository.GetDepartments();
        foreach (var d in depts)
        {
            Console.WriteLine($"  [{d.Id}] {d.Name}");
        }
        student.DepartmentId = ConsoleHelper.ReadInt("Department ID", 1, depts.Count, student.DepartmentId);
        student.DepartmentName = depts.First(d => d.Id == student.DepartmentId).Name;

        Console.WriteLine("\nAcademic Year: [1] Freshman  [2] Sophomore  [3] Junior  [4] Senior");
        student.Year = (AcademicYear)ConsoleHelper.ReadInt("Academic Year", 1, 4, (int)student.Year);

        student.IsActive = ConsoleHelper.ReadBool("Is student actively enrolled?", student.IsActive);

        if (_repository.Update(student))
        {
            RefreshLinqService();
            ConsoleHelper.PrintSuccess($"Student '{student.FullName}' updated successfully.");
        }
        else
        {
            ConsoleHelper.PrintError("Failed to update student.");
        }

        ConsoleHelper.Pause();
    }

    private void EnrollStudentInCourse()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("ENROLL IN COURSE & RECORD GRADE");
        ConsoleHelper.PrintLinqTag("Any() / FirstOrDefault()", "Validates existing enrollments and matches course");

        var id = ConsoleHelper.ReadInt("Enter Student ID");
        var student = _repository.GetById(id);
        if (student == null)
        {
            ConsoleHelper.PrintError($"Student with ID {id} not found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine($"\nStudent: {student.FullName} ({student.DepartmentName})");
        Console.WriteLine("\nAvailable Courses:");
        var courses = _repository.GetCourses();
        for (int i = 0; i < courses.Count; i++)
        {
            var c = courses[i];
            Console.WriteLine($"  [{i + 1}] {c.CourseId} - {c.Title} ({c.Credits} Credits)");
        }

        var courseIndex = ConsoleHelper.ReadInt("Select course number", 1, courses.Count) - 1;
        var selectedCourse = courses[courseIndex];

        var existingEnrollment = student.Enrollments.FirstOrDefault(e => e.CourseId.Equals(selectedCourse.CourseId, StringComparison.OrdinalIgnoreCase));
        if (existingEnrollment != null)
        {
            ConsoleHelper.PrintWarning($"Student is already enrolled in {selectedCourse.Title} with score {existingEnrollment.Score}.");
            if (!ConsoleHelper.ReadBool("Do you want to update the score?", true))
            {
                return;
            }

            var newScore = ConsoleHelper.ReadDouble("Enter updated score (0 - 100)", 0.0, 100.0, existingEnrollment.Score);
            existingEnrollment.Score = newScore;
        }
        else
        {
            var score = ConsoleHelper.ReadDouble("Enter initial score (0 - 100)", 0.0, 100.0, 85.0);
            student.Enrollments.Add(new Enrollment(selectedCourse.CourseId, selectedCourse.Title, score, selectedCourse.Credits));
        }

        _repository.Update(student);
        RefreshLinqService();
        ConsoleHelper.PrintSuccess($"Enrollment recorded! Updated GPA: {student.Gpa:F2}");
        ConsoleHelper.Pause();
    }

    private void DeleteStudent()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("DELETE STUDENT RECORD");
        ConsoleHelper.PrintLinqTag("Remove() / Any()", "Removes student from underlying collection and file storage");

        var id = ConsoleHelper.ReadInt("Enter Student ID to delete");
        var student = _repository.GetById(id);

        if (student == null)
        {
            ConsoleHelper.PrintError($"Student with ID {id} not found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Warning: You are about to delete '{student.FullName}' (ID: {student.Id}).");
        Console.ResetColor();

        if (ConsoleHelper.ReadBool("Are you sure you want to delete this student?", false))
        {
            if (_repository.Delete(id))
            {
                RefreshLinqService();
                ConsoleHelper.PrintSuccess("Student record deleted successfully.");
            }
            else
            {
                ConsoleHelper.PrintError("Failed to delete student.");
            }
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    #endregion

    #region 2. LINQ Search & Filtering

    private void SearchAndFilterMenu()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("LINQ SEARCH & FILTERING (WHERE & PREDICATES)");
            Console.WriteLine("  1. Search by Name, Email, or ID (LINQ Where + Contains)");
            Console.WriteLine("  2. Filter by Department (LINQ Where)");
            Console.WriteLine("  3. Filter by Academic Year (LINQ Where)");
            Console.WriteLine("  4. Filter by Minimum GPA Threshold (LINQ Where + OrderByDescending)");
            Console.WriteLine("  5. Multi-Criteria Advanced Query (LINQ Dynamic Where)");
            Console.WriteLine("  0. Back to Main Menu");
            Console.WriteLine(new string('-', 78));

            var choice = ConsoleHelper.ReadInt("Select an option (0-5)", 0, 5);
            switch (choice)
            {
                case 1:
                    SearchByKeyword();
                    break;
                case 2:
                    FilterByDepartment();
                    break;
                case 3:
                    FilterByYear();
                    break;
                case 4:
                    FilterByGpa();
                    break;
                case 5:
                    AdvancedFilter();
                    break;
                case 0:
                    back = true;
                    break;
            }
        }
    }

    private void SearchByKeyword()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("KEYWORD SEARCH");
        ConsoleHelper.PrintLinqTag("Where() with Contains()", "s => s.FullName.Contains(term) || s.Email.Contains(term)");

        var keyword = ConsoleHelper.ReadString("Enter search keyword (name, email, or ID)");
        var results = _linqService.SearchStudents(searchTerm: keyword);

        ConsoleHelper.PrintStudentTable(results);
        ConsoleHelper.Pause();
    }

    private void FilterByDepartment()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("FILTER BY DEPARTMENT");
        ConsoleHelper.PrintLinqTag("Where()", "s => s.DepartmentId == selectedDeptId");

        var depts = _repository.GetDepartments();
        foreach (var d in depts)
        {
            Console.WriteLine($"  [{d.Id}] {d.Name} ({d.Code})");
        }

        var deptId = ConsoleHelper.ReadInt("Select Department ID", 1, depts.Count);
        var results = _linqService.SearchStudents(departmentId: deptId);

        ConsoleHelper.PrintStudentTable(results);
        ConsoleHelper.Pause();
    }

    private void FilterByYear()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("FILTER BY ACADEMIC YEAR");
        ConsoleHelper.PrintLinqTag("Where()", "s => s.Year == selectedYear");

        Console.WriteLine("Select Academic Year: [1] Freshman  [2] Sophomore  [3] Junior  [4] Senior");
        var yearInt = ConsoleHelper.ReadInt("Academic Year", 1, 4);
        var results = _linqService.SearchStudents(year: (AcademicYear)yearInt);

        ConsoleHelper.PrintStudentTable(results);
        ConsoleHelper.Pause();
    }

    private void FilterByGpa()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("FILTER BY GPA THRESHOLD");
        ConsoleHelper.PrintLinqTag("Where() & OrderByDescending()", "s => s.Gpa >= minGpa");

        var minGpa = ConsoleHelper.ReadDouble("Enter minimum GPA threshold (0.0 - 4.0)", 0.0, 4.0, 3.0);
        var results = _linqService.SearchStudents(minGpa: minGpa)
                                 .OrderByDescending(s => s.Gpa)
                                 .ToList();

        ConsoleHelper.PrintStudentTable(results);
        ConsoleHelper.Pause();
    }

    private void AdvancedFilter()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("ADVANCED MULTI-CRITERIA FILTER");
        ConsoleHelper.PrintLinqTag("Queryable Chaining", "Chained .Where() calls conditionally composed");

        var keyword = ConsoleHelper.ReadString("Search keyword (optional, press Enter to skip)", false);

        Console.WriteLine("\nDepartments: [0] Any");
        var depts = _repository.GetDepartments();
        foreach (var d in depts) Console.WriteLine($"  [{d.Id}] {d.Name}");
        var deptId = ConsoleHelper.ReadInt("Department ID", 0, depts.Count, 0);

        Console.WriteLine("\nAcademic Year: [0] Any  [1] Freshman  [2] Sophomore  [3] Junior  [4] Senior");
        var yearInt = ConsoleHelper.ReadInt("Year", 0, 4, 0);

        var minGpa = ConsoleHelper.ReadDouble("Minimum GPA (0.0 to 4.0, 0 for any)", 0.0, 4.0, 0.0);
        var activeOnly = ConsoleHelper.ReadBool("Show active students only?", true);

        var results = _linqService.SearchStudents(
            searchTerm: string.IsNullOrEmpty(keyword) ? null : keyword,
            departmentId: deptId == 0 ? null : deptId,
            year: yearInt == 0 ? null : (AcademicYear)yearInt,
            minGpa: minGpa > 0.0 ? minGpa : null,
            isActive: activeOnly ? true : null
        );

        ConsoleHelper.PrintStudentTable(results);
        ConsoleHelper.Pause();
    }

    #endregion

    #region 3. LINQ Sorting Playground

    private void SortingPlaygroundMenu()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("LINQ DYNAMIC SORTING PLAYGROUND");
        ConsoleHelper.PrintLinqTag("OrderBy(), OrderByDescending(), ThenBy()", "Applies secondary and tertiary sort keys");

        Console.WriteLine("Choose Primary Sort Field:");
        Console.WriteLine("  1. Student Name (LastName, then FirstName)");
        Console.WriteLine("  2. GPA (Highest / Lowest, secondary: Name)");
        Console.WriteLine("  3. Age (secondary: GPA)");
        Console.WriteLine("  4. Department (secondary: GPA)");
        Console.WriteLine("  5. Academic Year (secondary: GPA)");
        Console.WriteLine("  6. Student ID");

        var sortFieldChoice = ConsoleHelper.ReadInt("Sort by", 1, 6, 2);
        var ascending = ConsoleHelper.ReadBool("Sort in Ascending order? (n for Descending)", false);

        string field = sortFieldChoice switch
        {
            1 => "name",
            2 => "gpa",
            3 => "age",
            4 => "department",
            5 => "year",
            _ => "id"
        };

        var sorted = _linqService.GetSortedStudents(field, ascending);

        Console.Clear();
        ConsoleHelper.PrintHeader($"SORTED BY: {field.ToUpper()} ({(ascending ? "ASC" : "DESC")})");
        ConsoleHelper.PrintStudentTable(sorted);
        ConsoleHelper.Pause();
    }

    #endregion

    #region 4. LINQ Analytics & Reports Dashboard

    private void AnalyticsDashboardMenu()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            ConsoleHelper.PrintHeader("LINQ ANALYTICS & REPORTS DASHBOARD");
            Console.WriteLine("  1. Department-wise Summary (LINQ GroupBy + Aggregates)");
            Console.WriteLine("  2. Top N Students by GPA (LINQ OrderByDescending + Take)");
            Console.WriteLine("  3. Honors Students by GPA Cutoff (LINQ TakeWhile)");
            Console.WriteLine("  4. Academic Year Distribution (LINQ GroupBy + ToDictionary)");
            Console.WriteLine("  5. University-Wide Grade Distribution (LINQ SelectMany + GroupBy)");
            Console.WriteLine("  6. Course Enrollment Analytics (LINQ Inner Join + GroupBy)");
            Console.WriteLine("  7. Consistent Achievers vs At-Risk Students (LINQ All & Any)");
            Console.WriteLine("  8. Overall University Aggregate Metrics (LINQ Sum, Min, Max, Average)");
            Console.WriteLine("  0. Back to Main Menu");
            Console.WriteLine(new string('-', 78));

            var choice = ConsoleHelper.ReadInt("Select an option (0-8)", 0, 8);
            switch (choice)
            {
                case 1:
                    ReportDepartmentSummary();
                    break;
                case 2:
                    ReportTopStudents();
                    break;
                case 3:
                    ReportTakeWhileHonors();
                    break;
                case 4:
                    ReportAcademicYearDistribution();
                    break;
                case 5:
                    ReportGradeDistribution();
                    break;
                case 6:
                    ReportCourseAnalytics();
                    break;
                case 7:
                    ReportAchieversAndAtRisk();
                    break;
                case 8:
                    ReportOverallMetrics();
                    break;
                case 0:
                    back = true;
                    break;
            }
        }
    }

    private void ReportDepartmentSummary()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("DEPARTMENT-WISE ANALYTICS");
        ConsoleHelper.PrintLinqTag("GroupBy() + Aggregates", ".GroupBy(s => s.Department).Select(g => new { Avg = g.Average(s => s.Gpa) })");

        var reports = _linqService.GetDepartmentReports();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 25) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 9) + "+" + new string('-', 9) + "+" + new string('-', 25) + "+");
        Console.WriteLine($"| {"Department",-23} | {"Students",-8} | {"Avg GPA",-8} | {"Max GPA",-7} | {"Min GPA",-7} | {"Top Student",-23} |");
        Console.WriteLine("+" + new string('-', 25) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 9) + "+" + new string('-', 9) + "+" + new string('-', 25) + "+");
        Console.ResetColor();

        foreach (var r in reports)
        {
            Console.WriteLine($"| {r.DepartmentName,-23} | {r.StudentCount,8} | {r.AverageGpa,8:F2} | {r.HighestGpa,7:F2} | {r.LowestGpa,7:F2} | {r.TopStudentName,-23} |");
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 25) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 9) + "+" + new string('-', 9) + "+" + new string('-', 25) + "+");
        Console.ResetColor();
        ConsoleHelper.Pause();
    }

    private void ReportTopStudents()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("TOP PERFORMING STUDENTS");
        ConsoleHelper.PrintLinqTag("OrderByDescending() + Take()", ".OrderByDescending(s => s.Gpa).Take(n)");

        var count = ConsoleHelper.ReadInt("Enter number of top students to display (1-20)", 1, 20, 5);
        var top = _linqService.GetTopStudents(count);

        ConsoleHelper.PrintStudentTable(top);
        ConsoleHelper.Pause();
    }

    private void ReportTakeWhileHonors()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("HONOR ROLL SLICE");
        ConsoleHelper.PrintLinqTag("OrderByDescending() + TakeWhile()", ".OrderByDescending(s => s.Gpa).TakeWhile(s => s.Gpa >= threshold)");

        var threshold = ConsoleHelper.ReadDouble("Enter cutoff GPA (e.g. 3.5)", 0.0, 4.0, 3.5);
        var honors = _linqService.GetStudentsAboveGpaThreshold(threshold);

        ConsoleHelper.PrintInfo($"Found {honors.Count} student(s) maintaining GPA >= {threshold:F2}.");
        ConsoleHelper.PrintStudentTable(honors);
        ConsoleHelper.Pause();
    }

    private void ReportAcademicYearDistribution()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("STUDENT ENROLLMENT BY ACADEMIC YEAR");
        ConsoleHelper.PrintLinqTag("GroupBy() + ToDictionary()", ".GroupBy(s => s.Year).ToDictionary(g => g.Key, g => g.ToList())");

        var distribution = _linqService.GetStudentsByAcademicYear();

        foreach (var pair in distribution)
        {
            ConsoleHelper.PrintSubHeader($"{pair.Key.ToString().ToUpper()} ({pair.Value.Count} Students)");
            ConsoleHelper.PrintStudentTable(pair.Value);
        }

        ConsoleHelper.Pause();
    }

    private void ReportGradeDistribution()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("GRADE LETTER DISTRIBUTION");
        ConsoleHelper.PrintLinqTag("SelectMany() + GroupBy()", "Flattens all courses via .SelectMany(s => s.Enrollments) and groups by letter grade");

        var distribution = _linqService.GetGradeDistribution();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 8) + "+" + new string('-', 10) + "+" + new string('-', 14) + "+" + new string('-', 35) + "+");
        Console.WriteLine($"| {"Grade",-6} | {"Count",-8} | {"Percentage",-12} | {"Visual Chart",-33} |");
        Console.WriteLine("+" + new string('-', 8) + "+" + new string('-', 10) + "+" + new string('-', 14) + "+" + new string('-', 35) + "+");
        Console.ResetColor();

        foreach (var g in distribution)
        {
            int barLength = (int)Math.Round(g.Percentage / 3);
            string bar = new string('#', barLength);

            var barColor = g.GradeLetter switch
            {
                "A" => ConsoleColor.Green,
                "B" => ConsoleColor.Cyan,
                "C" => ConsoleColor.Yellow,
                _ => ConsoleColor.Red
            };

            Console.Write($"| {g.GradeLetter,-6} | {g.Count,8} | {g.Percentage,10:F1}% | ");
            Console.ForegroundColor = barColor;
            Console.Write($"{bar,-33}");
            Console.ResetColor();
            Console.WriteLine(" |");
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 8) + "+" + new string('-', 10) + "+" + new string('-', 14) + "+" + new string('-', 35) + "+");
        Console.ResetColor();
        ConsoleHelper.Pause();
    }

    private void ReportCourseAnalytics()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("COURSE ENROLLMENT ANALYTICS");
        ConsoleHelper.PrintLinqTag("Join() + SelectMany() + GroupBy()", "Inner join between Course definitions and student enrollment records");

        var analytics = _linqService.GetCourseAnalytics();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 10) + "+" + new string('-', 33) + "+" + new string('-', 10) + "+" + new string('-', 11) + "+" + new string('-', 11) + "+" + new string('-', 11) + "+");
        Console.WriteLine($"| {"Code",-8} | {"Course Title",-31} | {"Enrolled",-8} | {"Avg Score",-9} | {"Max Score",-9} | {"Min Score",-9} |");
        Console.WriteLine("+" + new string('-', 10) + "+" + new string('-', 33) + "+" + new string('-', 10) + "+" + new string('-', 11) + "+" + new string('-', 11) + "+" + new string('-', 11) + "+");
        Console.ResetColor();

        foreach (var c in analytics)
        {
            Console.WriteLine($"| {c.CourseId,-8} | {c.CourseTitle,-31} | {c.EnrolledStudents,8} | {c.AverageScore,9:F1} | {c.HighestScore,9:F1} | {c.LowestScore,9:F1} |");
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 10) + "+" + new string('-', 33) + "+" + new string('-', 10) + "+" + new string('-', 11) + "+" + new string('-', 11) + "+" + new string('-', 11) + "+");
        Console.ResetColor();
        ConsoleHelper.Pause();
    }

    private void ReportAchieversAndAtRisk()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("HIGH ACHIEVERS VS. AT-RISK STUDENTS");

        ConsoleHelper.PrintLinqTag("All()", ".Where(s => s.Enrollments.All(e => e.Score >= 80))");
        var achievers = _linqService.GetConsistentHighAchievers(80.0);
        ConsoleHelper.PrintSubHeader($"Consistent High Achievers (All Scores >= 80) [{achievers.Count}]");
        ConsoleHelper.PrintStudentTable(achievers);

        ConsoleHelper.PrintLinqTag("Any()", ".Where(s => s.Enrollments.Any(e => e.Score < 65) || s.Gpa < 2.0)");
        var atRisk = _linqService.GetAtRiskStudents(65.0);
        ConsoleHelper.PrintSubHeader($"Students Needing Academic Support (Any Score < 65 or GPA < 2.0) [{atRisk.Count}]");
        ConsoleHelper.PrintStudentTable(atRisk);

        ConsoleHelper.Pause();
    }

    private void ReportOverallMetrics()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("OVERALL UNIVERSITY METRICS");
        ConsoleHelper.PrintLinqTag("Sum(), Average(), Min(), Max(), Count()", "Aggregate functions executing across all student collections");

        var (total, avgGpa, maxGpa, minGpa, totalCredits) = _linqService.GetOverallStatistics();

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n  Total Enrolled Students : {total}");
        Console.WriteLine($"  University Average GPA  : {avgGpa:F2} / 4.00");
        Console.WriteLine($"  Highest Recorded GPA    : {maxGpa:F2}");
        Console.WriteLine($"  Lowest Recorded GPA     : {minGpa:F2}");
        Console.WriteLine($"  Total Credits Completed : {totalCredits} Credits");
        Console.ResetColor();

        ConsoleHelper.Pause();
    }

    #endregion

    #region 5. Set Operations

    private void SetOperationsMenu()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("LINQ SET OPERATIONS: COURSE COMPARISON");
        ConsoleHelper.PrintLinqTag("Intersect(), Union(), Except(), Distinct()", "Performs mathematical set comparisons between collections");

        var courses = _repository.GetCourses();
        Console.WriteLine("Available Courses:");
        for (int i = 0; i < courses.Count; i++)
        {
            var c = courses[i];
            Console.WriteLine($"  [{i + 1}] {c.CourseId} - {c.Title}");
        }

        var idx1 = ConsoleHelper.ReadInt("Select Course 1 number", 1, courses.Count, 1) - 1;
        var idx2 = ConsoleHelper.ReadInt("Select Course 2 number", 1, courses.Count, 7) - 1;

        var c1 = courses[idx1];
        var c2 = courses[idx2];

        ConsoleHelper.PrintSubHeader($"Comparing '{c1.CourseId} ({c1.Title})' vs. '{c2.CourseId} ({c2.Title})'");

        // 1. Intersect: Enrolled in BOTH
        ConsoleHelper.PrintLinqTag("Intersect()", "Students enrolled in BOTH courses");
        var both = _linqService.GetStudentsInBothCourses(c1.CourseId, c2.CourseId);
        Console.WriteLine($"-> Students taking BOTH courses [{both.Count}]:");
        ConsoleHelper.PrintStudentTable(both);

        // 2. Except: Enrolled in Course 1 but NOT Course 2
        ConsoleHelper.PrintLinqTag("Except()", $"Students in {c1.CourseId} but NOT in {c2.CourseId}");
        var course1Only = _linqService.GetStudentsInCourse1NotCourse2(c1.CourseId, c2.CourseId);
        Console.WriteLine($"-> Students taking ONLY {c1.CourseId} and NOT {c2.CourseId} [{course1Only.Count}]:");
        ConsoleHelper.PrintStudentTable(course1Only);

        // 3. Union: Enrolled in EITHER course
        ConsoleHelper.PrintLinqTag("Union()", "Unique students enrolled in EITHER Course 1 OR Course 2");
        var either = _linqService.GetStudentsInEitherCourse(c1.CourseId, c2.CourseId);
        Console.WriteLine($"-> Combined unique students taking EITHER course [{either.Count}]:");
        ConsoleHelper.PrintStudentTable(either);

        ConsoleHelper.Pause();
    }

    #endregion

    #region 6. Data Management

    private void DataManagementMenu()
    {
        Console.Clear();
        ConsoleHelper.PrintHeader("DATA MANAGEMENT & STORAGE");

        Console.WriteLine("  1. Save Current Student Records to 'students.json'");
        Console.WriteLine("  2. Reset All Data to Original Demonstration Seed Data");
        Console.WriteLine("  0. Back to Main Menu");
        Console.WriteLine(new string('-', 78));

        var choice = ConsoleHelper.ReadInt("Select an option (0-2)", 0, 2);
        switch (choice)
        {
            case 1:
                _repository.SaveData();
                ConsoleHelper.PrintSuccess("Student records successfully saved to 'students.json'.");
                ConsoleHelper.Pause();
                break;
            case 2:
                if (ConsoleHelper.ReadBool("Are you sure you want to reset all records to initial seed data?", false))
                {
                    _repository.ResetToSeedData();
                    RefreshLinqService();
                    ConsoleHelper.PrintSuccess("Records successfully reset to seed data!");
                }
                else
                {
                    ConsoleHelper.PrintInfo("Reset cancelled.");
                }
                ConsoleHelper.Pause();
                break;
            case 0:
                break;
        }
    }

    #endregion
}
