using EmployeeMgmt.Core;

namespace EmployeeMgmt.Tests;

public class AsyncCancellationTests
{
    [Fact]
    public async Task LoadEmployeesAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var repo = new InMemoryEmployeeRepository();
        var emp = new Contractor(1, "Test", 50m, "Agency", 12);
        repo.Add(emp);
        
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => repo.LoadEmployeesAsync(cts.Token));
    }
}
