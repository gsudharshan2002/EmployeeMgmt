using System.ComponentModel.DataAnnotations;

namespace EmployeeMgmt.Application.Employees;

public sealed record EmployeeDto(int Id, string Name, decimal Salary, int? DepartmentId, string? DepartmentName);

public sealed class EmployeeRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal Salary { get; init; }

    [Range(1, int.MaxValue)]
    public int? DepartmentId { get; init; }
}

public sealed record EmployeeQuery(
    string? Search,
    int? DepartmentId,
    string SortBy = "name",
    bool Descending = false,
    int Page = 1,
    int PageSize = 20);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<EmployeeDto> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken);
    Task<EmployeeDto?> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
