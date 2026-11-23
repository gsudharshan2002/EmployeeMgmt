using EmployeeMgmt.Core;

namespace EmployeeMgmt.Application.Employees;

public sealed class EmployeeService(IEmployeeStore store) : IEmployeeService
{
    public async Task<PagedResult<EmployeeDto>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        ValidateQuery(query);
        var employees = (await store.GetAllAsync(cancellationToken)).AsEnumerable();

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
        var page = employees.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        var items = await Task.WhenAll(page.Select(employee => ToDto(employee, cancellationToken)));
        return new PagedResult<EmployeeDto>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await store.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : await ToDto(employee, cancellationToken);
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken)
    {
        await EnsureDepartmentExists(request.DepartmentId, cancellationToken);
        var employee = new Employee(0, request.Name.Trim(), request.Salary, request.DepartmentId);
        await store.AddAsync(employee, cancellationToken);
        return await ToDto(employee, cancellationToken);
    }

    public async Task<EmployeeDto?> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await store.GetByIdAsync(id, cancellationToken);
        if (employee is null)
            return null;
        await EnsureDepartmentExists(request.DepartmentId, cancellationToken);
        employee.Update(request.Name.Trim(), request.Salary, request.DepartmentId);
        await store.UpdateAsync(employee, cancellationToken);
        return await ToDto(employee, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        store.DeleteAsync(id, cancellationToken);

    private async Task EnsureDepartmentExists(int? departmentId, CancellationToken cancellationToken)
    {
        if (departmentId is int id && await store.GetDepartmentAsync(id, cancellationToken) is null)
            throw new ArgumentException($"Department {id} does not exist.", nameof(departmentId));
    }

    private async Task<EmployeeDto> ToDto(Employee employee, CancellationToken cancellationToken)
    {
        var department = employee.DepartmentId is int id
            ? await store.GetDepartmentAsync(id, cancellationToken)
            : null;
        return new EmployeeDto(employee.Id, employee.Name, employee.Salary, employee.DepartmentId, department?.Name);
    }

    private static void ValidateQuery(EmployeeQuery query)
    {
        if (query.Page < 1)
            throw new ArgumentException("Page must be greater than zero.", nameof(query));
        if (query.PageSize is < 1 or > 100)
            throw new ArgumentException("Page size must be between 1 and 100.", nameof(query));
        if (query.SortBy is not ("name" or "salary" or "id"))
            throw new ArgumentException("SortBy must be name, salary, or id.", nameof(query));
    }
}
