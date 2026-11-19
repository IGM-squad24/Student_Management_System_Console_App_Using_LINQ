using System.Text;
using StudentManagementSystem.Data;
using StudentManagementSystem.UI;

namespace StudentManagementSystem;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Student Management System - Powered by LINQ";

        var repository = new StudentRepository("students.json");
        var menuHandler = new MenuHandler(repository);

        menuHandler.Run();
    }
}
