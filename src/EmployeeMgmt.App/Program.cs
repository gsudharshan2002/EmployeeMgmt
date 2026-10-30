using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeMgmt.App;

public sealed class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Department { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
}

public sealed class EmployeeRepository
{
    private readonly List<Employee> _employees = new();

    public EmployeeRepository()
    {
        Seed();
    }

    private void Seed()
    {
        _employees.Add(new Employee { Id = 1, Name = "Alice", Department = "Engineering", Salary = 90000m, HireDate = new DateTime(2020, 3, 15) });
        _employees.Add(new Employee { Id = 2, Name = "Bob", Department = "HR", Salary = 65000m, HireDate = new DateTime(2019, 7, 1) });
        _employees.Add(new Employee { Id = 3, Name = "Carol", Department = "Engineering", Salary = 95000m, HireDate = new DateTime(2021, 1, 10) });
        _employees.Add(new Employee { Id = 4, Name = "David", Department = "Marketing", Salary = 70000m, HireDate = new DateTime(2018, 11, 5) });
        _employees.Add(new Employee { Id = 5, Name = "Eve", Department = "Sales", Salary = 72000m, HireDate = new DateTime(2022, 2, 20) });
    }

    public void AddEmployee(Employee emp)
    {
        if (string.IsNullOrWhiteSpace(emp.Name))
        {
            throw new ArgumentException("Employee name cannot be empty.", nameof(emp.Name));
        }

        if (_employees.Exists(e => e.Id == emp.Id))
        {
            throw new InvalidOperationException($"Duplicate employee ID: {emp.Id}");
        }

        _employees.Add(emp);
    }

    public Employee? FindById(int id)
    {
        foreach (var emp in _employees)
        {
            if (emp.Id == id)
            {
                return emp;
            }
        }
        return null;
    }

    public string PrintAll()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Employee Records:");
        sb.AppendLine(new string('-', 40));

        foreach (var emp in _employees)
        {
            sb.AppendLine($"Id={emp.Id}, Name={emp.Name}, Department={emp.Department ?? "N/A"}, Salary={emp.Salary:C}, HireDate={emp.HireDate:yyyy-MM-dd}");
        }

        sb.AppendLine($"Total count: {_employees.Count}");
        return sb.ToString();
    }

    public decimal AverageSalary()
    {
        if (_employees.Count == 0)
        {
            throw new InvalidOperationException("No employees to compute average salary.");
        }

        decimal sum = 0m;
        foreach (var emp in _employees)
        {
            sum += emp.Salary;
        }

        return sum / _employees.Count;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var repo = new EmployeeRepository();

        Console.WriteLine(repo.PrintAll());

        Console.WriteLine($"Average Salary: {repo.AverageSalary():C}");

        Console.WriteLine("--- FindById tests ---");
        var found = repo.FindById(3);
        Console.WriteLine(found is not null ? $"Found: {found.Name}" : "Not found");
        var missing = repo.FindById(99);
        Console.WriteLine(missing is not null ? $"Found: {missing.Name}" : "Not found (nullable)");

        Console.WriteLine("--- AddEmployee tests ---");
        try
        {
            repo.AddEmployee(new Employee { Id = 1, Name = "Duplicate", Department = "IT", Salary = 50000m, HireDate = DateTime.Today });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Duplicate ID] Caught: {ex.Message}");
        }

        try
        {
            repo.AddEmployee(new Employee { Id = 6, Name = "", Department = "IT", Salary = 50000m, HireDate = DateTime.Today });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Empty Name] Caught: {ex.Message}");
        }

        try
        {
            repo.AddEmployee(new Employee { Id = 6, Name = "Frank", Department = "IT", Salary = 50000m, HireDate = DateTime.Today });
            Console.WriteLine("Added Frank successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Add Frank] Caught: {ex.Message}");
        }

        Console.WriteLine(repo.PrintAll());
    }
}
