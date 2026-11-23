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

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name cannot be empty", nameof(name));
        Name = name;
    }

    public void AddEmployee(Employee emp)
    {
        if (emp is null) throw new ArgumentNullException(nameof(emp));
        _employees.Add(emp);
    }

    public void RemoveEmployee(Employee employee) => _employees.Remove(employee);

    public void AssignId(int id)
    {
        if (Id != 0 || id < 1)
            throw new InvalidOperationException("A department ID can only be assigned once.");
        Id = id;
    }

    public string Describe() => $"Department {Id}: {Name}, EmployeeCount: {_employees.Count}";
}
