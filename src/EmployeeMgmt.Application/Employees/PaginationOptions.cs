namespace EmployeeMgmt.Application.Employees;

public sealed class PaginationOptions
{
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;
}
