using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data;

public static class SeedData
{
    public static List<Department> GetDepartments() => new()
    {
        new Department(1, "Computer Science", "CS"),
        new Department(2, "Electrical Engineering", "EE"),
        new Department(3, "Mathematics", "MATH"),
        new Department(4, "Business Administration", "BUS"),
        new Department(5, "Mechanical Engineering", "ME")
    };

    public static List<Course> GetCourses() => new()
    {
        new Course("CS101", "Introduction to Programming", 4, 1),
        new Course("CS201", "Data Structures & Algorithms", 4, 1),
        new Course("CS301", "Database Management Systems", 3, 1),
        new Course("CS401", "Artificial Intelligence", 3, 1),
        new Course("EE101", "Circuit Analysis", 3, 2),
        new Course("EE201", "Digital Logic Design", 4, 2),
        new Course("MATH101", "Calculus I", 4, 3),
        new Course("MATH201", "Linear Algebra", 3, 3),
        new Course("MATH301", "Probability & Statistics", 3, 3),
        new Course("BUS101", "Principles of Management", 3, 4),
        new Course("BUS201", "Financial Accounting", 3, 4),
        new Course("ME101", "Engineering Mechanics", 4, 5)
    };

    public static List<Student> GetStudents()
    {
        var s1 = new Student(101, "Alice", "Johnson", "alice.j@university.edu", 20, Gender.Female, 1, "Computer Science", AcademicYear.Sophomore, new DateTime(2023, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("CS101", "Introduction to Programming", 95, 4),
                new("CS201", "Data Structures & Algorithms", 91, 4),
                new("MATH101", "Calculus I", 88, 4)
            }
        };

        var s2 = new Student(102, "Bob", "Smith", "bob.s@university.edu", 22, Gender.Male, 1, "Computer Science", AcademicYear.Senior, new DateTime(2021, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("CS201", "Data Structures & Algorithms", 84, 4),
                new("CS301", "Database Management Systems", 78, 3),
                new("CS401", "Artificial Intelligence", 92, 3),
                new("MATH201", "Linear Algebra", 81, 3)
            }
        };

        var s3 = new Student(103, "Catherine", "Davis", "catherine.d@university.edu", 19, Gender.Female, 2, "Electrical Engineering", AcademicYear.Freshman, new DateTime(2024, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("EE101", "Circuit Analysis", 89, 3),
                new("MATH101", "Calculus I", 94, 4),
                new("CS101", "Introduction to Programming", 82, 4)
            }
        };

        var s4 = new Student(104, "David", "Miller", "david.m@university.edu", 21, Gender.Male, 2, "Electrical Engineering", AcademicYear.Junior, new DateTime(2022, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("EE101", "Circuit Analysis", 74, 3),
                new("EE201", "Digital Logic Design", 68, 4),
                new("MATH201", "Linear Algebra", 70, 3)
            }
        };

        var s5 = new Student(105, "Emma", "Wilson", "emma.w@university.edu", 20, Gender.Female, 3, "Mathematics", AcademicYear.Sophomore, new DateTime(2023, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("MATH101", "Calculus I", 98, 4),
                new("MATH201", "Linear Algebra", 95, 3),
                new("MATH301", "Probability & Statistics", 93, 3),
                new("CS101", "Introduction to Programming", 90, 4)
            }
        };

        var s6 = new Student(106, "Frank", "Taylor", "frank.t@university.edu", 23, Gender.Male, 4, "Business Administration", AcademicYear.Senior, new DateTime(2021, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("BUS101", "Principles of Management", 85, 3),
                new("BUS201", "Financial Accounting", 88, 3),
                new("MATH301", "Probability & Statistics", 76, 3)
            }
        };

        var s7 = new Student(107, "Grace", "Brown", "grace.b@university.edu", 19, Gender.Female, 1, "Computer Science", AcademicYear.Freshman, new DateTime(2024, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("CS101", "Introduction to Programming", 92, 4),
                new("MATH101", "Calculus I", 86, 4)
            }
        };

        var s8 = new Student(108, "Henry", "White", "henry.w@university.edu", 21, Gender.Male, 4, "Business Administration", AcademicYear.Junior, new DateTime(2022, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("BUS101", "Principles of Management", 79, 3),
                new("BUS201", "Financial Accounting", 65, 3)
            }
        };

        var s9 = new Student(109, "Ivy", "Anderson", "ivy.a@university.edu", 20, Gender.Female, 5, "Mechanical Engineering", AcademicYear.Sophomore, new DateTime(2023, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("ME101", "Engineering Mechanics", 87, 4),
                new("MATH101", "Calculus I", 83, 4),
                new("MATH201", "Linear Algebra", 79, 3)
            }
        };

        var s10 = new Student(110, "Jack", "Robinson", "jack.r@university.edu", 22, Gender.Male, 1, "Computer Science", AcademicYear.Senior, new DateTime(2021, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("CS101", "Introduction to Programming", 62, 4),
                new("CS201", "Data Structures & Algorithms", 58, 4),
                new("MATH101", "Calculus I", 64, 4)
            }
        };

        var s11 = new Student(111, "Kelly", "Martinez", "kelly.m@university.edu", 21, Gender.Female, 3, "Mathematics", AcademicYear.Junior, new DateTime(2022, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("MATH101", "Calculus I", 91, 4),
                new("MATH301", "Probability & Statistics", 89, 3),
                new("CS201", "Data Structures & Algorithms", 85, 4)
            }
        };

        var s12 = new Student(112, "Leo", "Clark", "leo.c@university.edu", 19, Gender.Male, 5, "Mechanical Engineering", AcademicYear.Freshman, new DateTime(2024, 9, 1))
        {
            Enrollments = new List<Enrollment>
            {
                new("ME101", "Engineering Mechanics", 91, 4),
                new("MATH101", "Calculus I", 78, 4)
            }
        };

        return new List<Student> { s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12 };
    }
}
