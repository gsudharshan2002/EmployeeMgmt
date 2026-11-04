using EmployeeMgmt.Core;

namespace EmployeeMgmt.Tests;

public class EmployeeQueriesTests
{
    private static List<Employee> GetTestEmployees()
    {
        var emp1 = new Contractor(1, "Alice", 75m, "TechStaff", 12);
        var emp2 = new Contractor(2, "Bob", 80m, "DevPro", 6);
        var manager = new Manager(3, "Carol", 120000m, 15000m);
        var emp3 = new Contractor(4, "David", 65m, "TechStaff", 12);
        var emp4 = new Contractor(5, "Eve", 70m, "DevPro", 9);
        
        return new List<Employee> { emp1, emp2, manager, emp3, emp4 };
    }

    [Fact]
    public void GetEmployeesEarningAbove_ThresholdFiltering_ReturnsOnlyHighEarners()
    {
        // Arrange
        var employees = GetTestEmployees();
        decimal threshold = 130000m;
        
        // Act
        var result = employees.GetEmployeesEarningAbove(threshold).ToList();
        
        // Assert
        Assert.Single(result);
        Assert.Equal("Alice", result[0].Name);
    }

    [Fact]
    public void ToSummaries_ProjectsToDto_ReturnsEmployeeSummaryRecords()
    {
        // Arrange
        var employees = GetTestEmployees();
        
        // Act
        var summaries = employees.ToSummaries().ToList();
        
        // Assert
        Assert.Equal(5, summaries.Count);
        Assert.All(summaries, s => Assert.IsType<EmployeeSummary>(s));
        Assert.Contains(summaries, s => s.Name == "Alice" && s.Role == "Contractor");
    }

    [Fact]
    public void SortBySalaryDescThenNameAsc_SortsCorrectly_ReturnsOrderedList()
    {
        // Arrange
        var employees = GetTestEmployees();
        
        // Act
        var sorted = employees.SortBySalaryDescThenNameAsc().ToList();
        
        // Assert
        Assert.Equal("Alice", sorted[0].Name); // Highest salary
        Assert.True(sorted[0].Salary >= sorted[1].Salary);
    }

    [Fact]
    public void FindByIdOrDefault_FoundAndNotFound_ReturnsEmployeeOrNull()
    {
        // Arrange
        var employees = GetTestEmployees();
        
        // Act
        var found = employees.FindByIdOrDefault(3);
        var notFound = employees.FindByIdOrDefault(99);
        
        // Assert
        Assert.NotNull(found);
        Assert.Equal("Carol", found.Name);
        Assert.Null(notFound);
    }

    [Fact]
    public void AggregateOperations_OnEmployeeList_ReturnsCorrectTotals()
    {
        // Arrange
        var employees = GetTestEmployees();
        
        // Act
        var total = employees.TotalSalary();
        var average = employees.AverageSalary();
        var highest = employees.HighestPaid();
        
        // Assert
        Assert.True(total > 0);
        Assert.True(average > 0);
        Assert.NotNull(highest);
        Assert.Equal(highest.Salary, employees.Max(e => e.Salary));
    }

    [Fact]
    public void GetEmployeesEarningAbove_EmptyList_ReturnsEmptyEnumerable()
    {
        // Arrange
        var employees = new List<Employee>();
        
        // Act
        var result = employees.GetEmployeesEarningAbove(50000m).ToList();
        
        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(1, "Alice")]
    [InlineData(2, "Bob")]
    [InlineData(4, "David")]
    public void FindByIdOrDefault_WithInlineData_ReturnsCorrectEmployee(int id, string expectedName)
    {
        // Arrange
        var employees = GetTestEmployees();
        
        // Act
        var result = employees.FindByIdOrDefault(id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedName, result.Name);
    }

    [Fact]
    public void AnyAll_ConditionsOnEmployees_ReturnsCorrectBooleans()
    {
        // Arrange
        var employees = GetTestEmployees();
        
        // Act
        var hasHighEarner = employees.HasAnyHighEarner(100000m);
        var allPositive = employees.AllHavePositiveSalary();
        
        // Assert
        Assert.True(hasHighEarner);
        Assert.True(allPositive);
    }
}
