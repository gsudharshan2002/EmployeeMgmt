using EmployeeMgmt.Core;

namespace EmployeeMgmt.Tests;

public class RegressionDefectTests
{
    [Fact]
    public void GetTopN_DefectRegression_TopNReturnsExactlyN()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Contractor(1, "A", 100m, "X", 12),
            new Contractor(2, "B", 90m, "X", 12),
            new Contractor(3, "C", 80m, "X", 12)
        };
        
        // Act
        var top2 = employees.GetTopN(2).ToList();
        
        // Assert - Regression: GetTopN was taking n-1 (off-by-one)
        Assert.Equal(2, top2.Count);
        Assert.Equal("A", top2[0].Name);
        Assert.Equal("B", top2[1].Name);
    }
}
