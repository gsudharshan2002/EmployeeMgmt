using EmployeeMgmt.Application.Employees;
using EmployeeMgmt.Core;

namespace EmployeeMgmt.Application.Departments;

public sealed class DepartmentService(IEmployeeStore store) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken) =>
        (await store.GetDepartmentsAsync(cancellationToken)).Select(ToDto).ToList();

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var department = await store.GetDepartmentAsync(id, cancellationToken);
        return department is null ? null : ToDto(department);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = new Department(0, request.Name.Trim());
        await store.AddDepartmentAsync(department, cancellationToken);
        return ToDto(department);
    }

    public async Task<DepartmentDto?> UpdateAsync(int id, DepartmentRequest request, CancellationToken cancellationToken)
    {
        var department = await store.GetDepartmentAsync(id, cancellationToken);
        if (department is null)
            return null;
        department.Update(request.Name.Trim());
        await store.UpdateDepartmentAsync(department, cancellationToken);
        return ToDto(department);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        await store.DeleteDepartmentAsync(id, cancellationToken);

    private static DepartmentDto ToDto(Department department) =>
        new(department.Id, department.Name, department.Employees.Count);
}
