namespace EmployeeMgmt.Core;

public sealed class Manager : Employee
{
    public List<Employee> Team { get; } = new();
    public decimal Bonus { get; private set; }

    public Manager(int id, string name, decimal salary, decimal bonus) : base(id, name, salary)
    {
        Bonus = bonus;
    }

    public void AddToTeam(Employee employee)
    {
        if (employee is null) throw new ArgumentNullException(nameof(employee));
        Team.Add(employee);
    }

    public override string Describe() => $"Manager {Id}: {Name}, TeamSize: {Team.Count}, TotalComp: {(Salary + Bonus):C}";
}
