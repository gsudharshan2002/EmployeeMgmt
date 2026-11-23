using System.ComponentModel.DataAnnotations;

namespace EmployeeMgmt.Application.Departments;

public sealed record DepartmentDto(int Id, string Name, int EmployeeCount);

public sealed class DepartmentRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;
}

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<DepartmentDto> CreateAsync(DepartmentRequest request, CancellationToken cancellationToken);
    Task<DepartmentDto?> UpdateAsync(int id, DepartmentRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
