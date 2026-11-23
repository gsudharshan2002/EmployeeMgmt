using EmployeeMgmt.Core;
using Microsoft.Extensions.Options;

namespace EmployeeMgmt.Application.Employees;

public sealed class EmployeeService(IEmployeeStore store, IOptions<PaginationOptions> paginationOptions) : IEmployeeService
{
    private readonly PaginationOptions _pagination = paginationOptions.Value;

    public async Task<PagedResult<EmployeeDto>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        ValidateQuery(query);
        var result = await store.GetAllAsync(query, cancellationToken);
        var items = await Task.WhenAll(result.Items.Select(employee => ToDto(employee, cancellationToken)));
        return new PagedResult<EmployeeDto>(items, result.Page, result.PageSize, result.TotalCount);
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
        var department = employee.Department ?? (employee.DepartmentId is int id
            ? await store.GetDepartmentAsync(id, cancellationToken)
            : null);
        return new EmployeeDto(employee.Id, employee.Name, employee.Salary, employee.DepartmentId, department?.Name);
    }

    private void ValidateQuery(EmployeeQuery query)
    {
        if (query.Page < 1)
            throw new ArgumentException("Page must be greater than zero.", nameof(query));
        if (query.PageSize < 1 || query.PageSize > _pagination.MaxPageSize)
            throw new ArgumentException($"Page size must be between 1 and {_pagination.MaxPageSize}.", nameof(query));
        if ((long)(query.Page - 1) * query.PageSize > int.MaxValue)
            throw new ArgumentException("Page is too large.", nameof(query));
        if (query.SortBy is not ("name" or "salary" or "id"))
            throw new ArgumentException("SortBy must be name, salary, or id.", nameof(query));
    }
}
