namespace EmployeeMgmt.Core;

public sealed class Department
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    private readonly List<Employee> _employees = new();

    public Department(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name cannot be empty", nameof(name));
        Id = id;
        Name = name;
    }

    public IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

    public void AddEmployee(Employee emp)
    {
        if (emp is null) throw new ArgumentNullException(nameof(emp));
        _employees.Add(emp);
    }

    public string Describe() => $"Department {Id}: {Name}, EmployeeCount: {_employees.Count}";
}
