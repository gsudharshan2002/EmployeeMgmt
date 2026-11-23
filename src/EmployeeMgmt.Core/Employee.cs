namespace EmployeeMgmt.Core;

public class Employee
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Salary { get; protected set; }
    public int? DepartmentId { get; private set; }
    public Department? Department { get; private set; }

    public Employee(int id, string name, decimal salary, int? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (salary < 0)
            throw new ArgumentOutOfRangeException(nameof(salary), "Salary cannot be negative.");

        Id = id;
        Name = name;
        Salary = salary;
        DepartmentId = departmentId;
    }

    public void Update(string name, decimal salary, int? departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (salary < 0)
            throw new ArgumentOutOfRangeException(nameof(salary), "Salary cannot be negative.");

        Name = name;
        Salary = salary;
        DepartmentId = departmentId;
    }

    public void AssignId(int id)
    {
        if (Id != 0 || id < 1)
            throw new InvalidOperationException("An employee ID can only be assigned once.");
        Id = id;
    }

    public virtual string Describe() => $"Employee {Id}: {Name}, Salary: {Salary:C}";
}
