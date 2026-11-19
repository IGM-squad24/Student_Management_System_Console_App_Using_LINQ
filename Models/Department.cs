namespace StudentManagementSystem.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public Department() { }

    public Department(int id, string name, string code)
    {
        Id = id;
        Name = name;
        Code = code;
    }
}
