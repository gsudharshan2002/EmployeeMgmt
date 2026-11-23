using EmployeeMgmt.Application.Employees;
using EmployeeMgmt.Core;
using EmployeeMgmt.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EmployeeMgmt.Tests;

public sealed class EmployeeApiPersistenceTests
{
    [Fact]
    public async Task EmployeeStore_FiltersOrdersAndPagesInSqlite()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<EmployeeDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var dbContext = new EmployeeDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        var store = new EfEmployeeStore(dbContext);

        var department = new Department(0, "Engineering");
        await store.AddDepartmentAsync(department, CancellationToken.None);
        await store.AddAsync(new Employee(0, "Alice", 90000m, department.Id), CancellationToken.None);
        await store.AddAsync(new Employee(0, "Alina", 70000m, department.Id), CancellationToken.None);
        await store.AddAsync(new Employee(0, "Bob", 80000m), CancellationToken.None);

        var result = await store.GetAllAsync(
            new EmployeeQuery("ali", department.Id, "salary", Descending: true, Page: 1, PageSize: 1),
            CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        var employee = Assert.Single(result.Items);
        Assert.Equal("Alice", employee.Name);
        Assert.Equal("Engineering", employee.Department?.Name);
    }

    [Fact]
    public async Task EmployeeService_RejectsInvalidPageSize()
    {
        var store = new InMemoryEmployeeStore();
        var service = new EmployeeService(store, Options.Create(new PaginationOptions()));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetAllAsync(new EmployeeQuery(null, null, PageSize: 101), CancellationToken.None));
    }

    [Fact]
    public async Task DepartmentService_RefusesToDeleteDepartmentWithEmployees()
    {
        var store = new InMemoryEmployeeStore();
        var departments = new EmployeeMgmt.Application.Departments.DepartmentService(store);
        var department = await departments.CreateAsync(new EmployeeMgmt.Application.Departments.DepartmentRequest
        {
            Name = "Engineering"
        }, CancellationToken.None);
        var employees = new EmployeeService(store, Options.Create(new PaginationOptions()));
        await employees.CreateAsync(new EmployeeRequest
        {
            Name = "Alice",
            Salary = 90000m,
            DepartmentId = department.Id
        }, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            departments.DeleteAsync(department.Id, CancellationToken.None));
    }
}
