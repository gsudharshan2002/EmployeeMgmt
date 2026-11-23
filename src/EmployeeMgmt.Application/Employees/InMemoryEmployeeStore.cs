using EmployeeMgmt.Core;

namespace EmployeeMgmt.Application.Employees;

public sealed class InMemoryEmployeeStore : IEmployeeStore
{
    private readonly Dictionary<int, Employee> _employees = new();
    private readonly Dictionary<int, Department> _departments = new();
    private int _nextEmployeeId = 1;
    private int _nextDepartmentId = 1;

    public Task<PagedResult<Employee>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<Employee> employees = _employees.Values;
        if (!string.IsNullOrWhiteSpace(query.Search))
            employees = employees.Where(employee => employee.Name.Contains(query.Search, StringComparison.OrdinalIgnoreCase));
        if (query.DepartmentId is int departmentId)
            employees = employees.Where(employee => employee.DepartmentId == departmentId);
        employees = query.SortBy.ToLowerInvariant() switch
        {
            "salary" => query.Descending ? employees.OrderByDescending(employee => employee.Salary).ThenBy(employee => employee.Id) : employees.OrderBy(employee => employee.Salary).ThenBy(employee => employee.Id),
            "id" => query.Descending ? employees.OrderByDescending(employee => employee.Id) : employees.OrderBy(employee => employee.Id),
            _ => query.Descending ? employees.OrderByDescending(employee => employee.Name).ThenBy(employee => employee.Id) : employees.OrderBy(employee => employee.Name).ThenBy(employee => employee.Id)
        };
        var totalCount = employees.Count();
        var items = employees.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        return Task.FromResult(new PagedResult<Employee>(items, query.Page, query.PageSize, totalCount));
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
            throw new InvalidOperationException($"Department {id} still has employees.");
        return Task.FromResult(_departments.Remove(id));
    }
}
