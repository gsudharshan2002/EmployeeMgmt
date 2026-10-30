using System.Linq;

namespace EmployeeMgmt.Core;

public sealed class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _employees = new();

    public int Count => _employees.Count;

    public void Add(Employee e)
    {
        if (e is null) throw new ArgumentNullException(nameof(e));
        if (_employees.Any(x => x.Id == e.Id))
            throw new InvalidOperationException($"Employee with Id {e.Id} already exists.");
        _employees.Add(e);
    }

    public Employee? GetById(int id)
    {
        return _employees.FirstOrDefault(e => e.Id == id);
    }

    public IEnumerable<Employee> GetAll()
    {
        return _employees;
    }

    public void Remove(int id)
    {
        var emp = GetById(id);
        if (emp is null) throw new InvalidOperationException($"Employee with Id {id} not found.");
        _employees.Remove(emp);
    }
}
