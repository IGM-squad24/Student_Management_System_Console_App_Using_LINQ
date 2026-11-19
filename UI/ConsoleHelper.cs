namespace StudentManagementSystem.UI;

public static class ConsoleHelper
{
    public static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=', 78));
        var padding = (78 - title.Length - 4) / 2;
        padding = Math.Max(0, padding);
        Console.WriteLine($"=={new string(' ', padding)}{title.ToUpper()}{new string(' ', 78 - title.Length - 4 - padding)}==");
        Console.WriteLine(new string('=', 78));
        Console.ResetColor();
    }

    public static void PrintSubHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n--- [ {title} ] {new string('-', Math.Max(2, 70 - title.Length))}");
        Console.ResetColor();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[SUCCESS] {message}");
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] {message}");
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    public static void PrintLinqTag(string linqOperator, string description)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write($"  [LINQ: {linqOperator}] ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(description);
        Console.ResetColor();
    }

    public static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nPress Enter to continue...");
        Console.ResetColor();
        Console.ReadLine();
    }

    public static string ReadString(string prompt, bool required = true, string defaultValue = "")
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (!string.IsNullOrEmpty(defaultValue))
                Console.Write($"{prompt} [{defaultValue}]: ");
            else
                Console.Write($"{prompt}: ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;
                if (!required) return string.Empty;
                PrintError("Value cannot be empty. Please try again.");
                continue;
            }
            return input;
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue, int? defaultValue = null)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (defaultValue.HasValue)
                Console.Write($"{prompt} [{defaultValue}]: ");
            else
                Console.Write($"{prompt}: ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                return defaultValue.Value;

            if (int.TryParse(input, out int result))
            {
                if (result >= min && result <= max)
                    return result;

                PrintError($"Please enter a number between {min} and {max}.");
            }
            else
            {
                PrintError("Invalid integer. Please try again.");
            }
        }
    }

    public static double ReadDouble(string prompt, double min = 0.0, double max = 100.0, double? defaultValue = null)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            if (defaultValue.HasValue)
                Console.Write($"{prompt} [{defaultValue}]: ");
            else
                Console.Write($"{prompt}: ");
            Console.ResetColor();

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                return defaultValue.Value;

            if (double.TryParse(input, out double result))
            {
                if (result >= min && result <= max)
                    return result;

                PrintError($"Please enter a value between {min} and {max}.");
            }
            else
            {
                PrintError("Invalid number. Please try again.");
            }
        }
    }

    public static bool ReadBool(string prompt, bool defaultYes = true)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{prompt} (y/n) [{(defaultYes ? "y" : "n")}]: ");
        Console.ResetColor();

        var input = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(input)) return defaultYes;
        return input == "y" || input == "yes";
    }

    public static void PrintStudentTable(IEnumerable<Models.Student> students)
    {
        var list = students.ToList();
        if (!list.Any())
        {
            PrintWarning("No student records found.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 6) + "+" + new string('-', 20) + "+" + new string('-', 23) + "+" + new string('-', 12) + "+" + new string('-', 6) + "+" + new string('-', 8) + "+");
        Console.WriteLine($"| {"ID",-4} | {"Name",-18} | {"Department",-21} | {"Year",-10} | {"GPA",-4} | {"Status",-6} |");
        Console.WriteLine("+" + new string('-', 6) + "+" + new string('-', 20) + "+" + new string('-', 23) + "+" + new string('-', 12) + "+" + new string('-', 6) + "+" + new string('-', 8) + "+");
        Console.ResetColor();

        foreach (var s in list)
        {
            var gpaColor = s.Gpa switch
            {
                >= 3.5 => ConsoleColor.Green,
                >= 2.5 => ConsoleColor.White,
                _ => ConsoleColor.Red
            };

            Console.Write($"| {s.Id,-4} | {Truncate(s.FullName, 18),-18} | {Truncate(s.DepartmentName, 21),-21} | {s.Year,-10} | ");
            Console.ForegroundColor = gpaColor;
            Console.Write($"{s.Gpa,4:F2}");
            Console.ResetColor();
            Console.Write(" | ");
            Console.ForegroundColor = s.IsActive ? ConsoleColor.DarkGreen : ConsoleColor.DarkGray;
            Console.Write($"{(s.IsActive ? "Active" : "Inact"),-6}");
            Console.ResetColor();
            Console.WriteLine(" |");
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("+" + new string('-', 6) + "+" + new string('-', 20) + "+" + new string('-', 23) + "+" + new string('-', 12) + "+" + new string('-', 6) + "+" + new string('-', 8) + "+");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Total: {list.Count} student(s) displayed.");
        Console.ResetColor();
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 2) + "..";
    }
}
