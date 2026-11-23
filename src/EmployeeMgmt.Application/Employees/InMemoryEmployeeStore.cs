using EmployeeMgmt.Core;

namespace EmployeeMgmt.Application.Employees;

public sealed class InMemoryEmployeeStore : IEmployeeStore
{
    private readonly Dictionary<int, Employee> _employees = new();
    private readonly Dictionary<int, Department> _departments = new();
    private int _nextEmployeeId = 1;
    private int _nextDepartmentId = 1;

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Employee>>(_employees.Values.ToList());
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _employees.TryGetValue(id, out var employee);
        return Task.FromResult(employee);
    }

    public Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        employee.AssignId(_nextEmployeeId++);
        _employees.Add(employee.Id, employee);
        if (employee.DepartmentId is int departmentId)
            _departments[departmentId].AddEmployee(employee);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Employee employee, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var department in _departments.Values)
            department.RemoveEmployee(employee);
        if (employee.DepartmentId is int departmentId)
            _departments[departmentId].AddEmployee(employee);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_employees.Remove(id, out var employee))
            return Task.FromResult(false);
        if (employee.DepartmentId is int departmentId && _departments.TryGetValue(departmentId, out var department))
            department.RemoveEmployee(employee);
        return Task.FromResult(true);
    }

    public Task<Department?> GetDepartmentAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _departments.TryGetValue(id, out var department);
        return Task.FromResult(department);
    }

    public Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Department>>(_departments.Values.ToList());
    }

    public Task AddDepartmentAsync(Department department, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        department.AssignId(_nextDepartmentId++);
        _departments.Add(department.Id, department);
        return Task.CompletedTask;
    }

    public Task UpdateDepartmentAsync(Department department, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_employees.Values.Any(employee => employee.DepartmentId == id))
            return Task.FromResult(false);
        return Task.FromResult(_departments.Remove(id));
    }
}
