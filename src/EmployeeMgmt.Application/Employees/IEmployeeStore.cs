using EmployeeMgmt.Core;

namespace EmployeeMgmt.Application.Employees;

public interface IEmployeeStore
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Employee employee, CancellationToken cancellationToken);
    Task UpdateAsync(Employee employee, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<Department?> GetDepartmentAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken);
    Task AddDepartmentAsync(Department department, CancellationToken cancellationToken);
    Task UpdateDepartmentAsync(Department department, CancellationToken cancellationToken);
    Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken);
}
