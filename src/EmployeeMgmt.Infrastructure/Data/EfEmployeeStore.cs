using EmployeeMgmt.Application.Employees;
using EmployeeMgmt.Core;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMgmt.Infrastructure.Data;

public sealed class EfEmployeeStore(EmployeeDbContext dbContext) : IEmployeeStore
{
    public async Task<PagedResult<Employee>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Employee> employees = dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Department);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            employees = employees.Where(employee => employee.Name.ToLower().Contains(search));
        }

        if (query.DepartmentId is int departmentId)
            employees = employees.Where(employee => employee.DepartmentId == departmentId);

        var totalCount = await employees.CountAsync(cancellationToken);
        employees = query.SortBy.ToLowerInvariant() switch
        {
            "salary" => query.Descending ? employees.OrderByDescending(employee => employee.Salary).ThenBy(employee => employee.Id) : employees.OrderBy(employee => employee.Salary).ThenBy(employee => employee.Id),
            "id" => query.Descending ? employees.OrderByDescending(employee => employee.Id) : employees.OrderBy(employee => employee.Id),
            _ => query.Descending ? employees.OrderByDescending(employee => employee.Name).ThenBy(employee => employee.Id) : employees.OrderBy(employee => employee.Name).ThenBy(employee => employee.Id)
        };

        var items = await employees.Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<Employee>(items, query.Page, query.PageSize, totalCount);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Employees.Include(employee => employee.Department)
            .FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FindAsync([id], cancellationToken);
        if (employee is null)
            return false;
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<Department?> GetDepartmentAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Departments.Include(department => department.Employees)
            .FirstOrDefaultAsync(department => department.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken) =>
        await dbContext.Departments.Include(department => department.Employees)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task AddDepartmentAsync(Department department, CancellationToken cancellationToken)
    {
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDepartmentAsync(Department department, CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments.FindAsync([id], cancellationToken);
        if (department is null)
            return false;
        var hasEmployees = await dbContext.Employees.AnyAsync(employee => employee.DepartmentId == id, cancellationToken);
        if (hasEmployees)
            throw new InvalidOperationException($"Department {id} still has employees.");
        dbContext.Departments.Remove(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
