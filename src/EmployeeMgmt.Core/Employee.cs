namespace EmployeeMgmt.Core;

public abstract class Employee
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Salary { get; protected set; }

    protected Employee(int id, string name, decimal salary)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Id = id;
        Name = name;
        Salary = salary;
    }

    public virtual string Describe() => $"Employee {Id}: {Name}, Salary: {Salary:C}";
}
