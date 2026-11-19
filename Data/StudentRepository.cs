using System.Text.Json;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data;

public class StudentRepository
{
    private readonly string _filePath;
    private List<Student> _students = new();
    private readonly List<Department> _departments;
    private readonly List<Course> _courses;

    public StudentRepository(string filePath = "students.json")
    {
        _filePath = filePath;
        _departments = SeedData.GetDepartments();
        _courses = SeedData.GetCourses();
        LoadData();
    }

    public List<Student> GetAll() => _students;
    public List<Department> GetDepartments() => _departments;
    public List<Course> GetCourses() => _courses;

    public Student? GetById(int id) =>
        _students.FirstOrDefault(s => s.Id == id);

    public bool Add(Student student)
    {
        if (_students.Any(s => s.Id == student.Id))
        {
            return false;
        }

        _students.Add(student);
        SaveData();
        return true;
    }

    public bool Update(Student updatedStudent)
    {
        var existing = _students.FirstOrDefault(s => s.Id == updatedStudent.Id);
        if (existing == null) return false;

        existing.FirstName = updatedStudent.FirstName;
        existing.LastName = updatedStudent.LastName;
        existing.Email = updatedStudent.Email;
        existing.Age = updatedStudent.Age;
        existing.Gender = updatedStudent.Gender;
        existing.DepartmentId = updatedStudent.DepartmentId;
        existing.DepartmentName = updatedStudent.DepartmentName;
        existing.Year = updatedStudent.Year;
        existing.IsActive = updatedStudent.IsActive;
        existing.Enrollments = updatedStudent.Enrollments;

        SaveData();
        return true;
    }

    public bool Delete(int id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student == null) return false;

        _students.Remove(student);
        SaveData();
        return true;
    }

    public int GetNextId() =>
        _students.Any() ? _students.Max(s => s.Id) + 1 : 101;

    public void ResetToSeedData()
    {
        _students = SeedData.GetStudents();
        SaveData();
    }

    public void SaveData()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_students, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Warning] Failed to save student records to disk: {ex.Message}");
        }
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<Student>>(json);
                if (loaded != null && loaded.Count > 0)
                {
                    _students = loaded;
                    return;
                }
            }
        }
        catch
        {
            // If corrupt or unable to read, fallback to seed
        }

        _students = SeedData.GetStudents();
        SaveData();
    }
}
