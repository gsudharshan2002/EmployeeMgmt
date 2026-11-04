using EmployeeMgmt.Core;

namespace EmployeeMgmt.Tests;

public class ComprehensiveTests
{
    [Fact]
    public void GetTopN_NormalData_ReturnsCorrectCount()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Contractor(1, "A", 100m, "X", 12),
            new Contractor(2, "B", 50m, "X", 12),
            new Contractor(3, "C", 75m, "X", 12)
        };
        
        // Act
        var top2 = employees.GetTopN(2).ToList();
        
        // Assert
        Assert.Equal(2, top2.Count);
        Assert.Equal("A", top2[0].Name);
    }
    
    [Fact]
    public void GetTopN_BoundarySingleItem_ReturnsOne()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Contractor(1, "Solo", 60m, "X", 12)
        };
        
        // Act
        var result = employees.GetTopN(1).ToList();
        
        // Assert
        Assert.Single(result);
    }
    
    [Fact]
    public void GetTopN_BoundaryEmptyList_ReturnsEmpty()
    {
        // Arrange
        var employees = new List<Employee>();
        
        // Act
        var result = employees.GetTopN(5).ToList();
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void GetDistinctRoles_NormalData_ReturnsUniqueRoles()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Contractor(1, "A", 50m, "X", 12),
            new Contractor(2, "B", 60m, "X", 12),
            new Manager(3, "C", 100000m, 10000m)
        };
        
        // Act
        var roles = employees.GetDistinctRoles().ToList();
        
        // Assert
        Assert.Equal(2, roles.Count);
        Assert.Contains("Contractor", roles);
        Assert.Contains("Manager", roles);
    }
    
    [Fact]
    public void AverageSalary_BoundaryEmptyList_ThrowsException()
    {
        // Arrange
        var employees = new List<Employee>();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => employees.AverageSalary());
    }
    
    [Fact]
    public void MaxSalary_BoundaryEmptyList_ThrowsException()
    {
        // Arrange
        var employees = new List<Employee>();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => employees.MaxSalary());
    }
    
    [Fact]
    public void HighestPaid_BoundaryEmptyList_ReturnsNull()
    {
        // Arrange
        var employees = new List<Employee>();
        
        // Act
        var result = employees.HighestPaid();
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void InMemoryRepository_InvalidDuplicateId_ThrowsException()
    {
        // Arrange
        var repo = new InMemoryEmployeeRepository();
        var emp1 = new Contractor(1, "A", 50m, "X", 12);
        repo.Add(emp1);
        var emp2 = new Contractor(1, "B", 60m, "Y", 12);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => repo.Add(emp2));
    }
    
    [Fact]
    public void InMemoryRepository_InvalidNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        var repo = new InMemoryEmployeeRepository();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => repo.Add(null!));
    }
    
    [Fact]
    public void Employee_InvalidEmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Contractor(1, "", 50m, "X", 12));
    }
    
    [Fact]
    public void Department_InvalidEmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Department(1, ""));
    }
    
    [Fact]
    public void Manager_AddToTeamComposition_WorksCorrectly()
    {
        // Arrange
        var manager = new Manager(1, "Boss", 100000m, 20000m);
        var emp = new Contractor(2, "Worker", 50m, "X", 12);
        
        // Act
        manager.AddToTeam(emp);
        
        // Assert
        Assert.Single(manager.Team);
        Assert.Equal("Worker", manager.Team[0].Name);
    }
}
