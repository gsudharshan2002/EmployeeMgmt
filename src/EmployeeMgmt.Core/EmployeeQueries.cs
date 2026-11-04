using System.Linq;

namespace EmployeeMgmt.Core;

public static class EmployeeQueries
{
    // 1. Where - employees earning above X
    public static IEnumerable<Employee> GetEmployeesEarningAbove(this IEnumerable<Employee> employees, decimal threshold)
    {
        return employees.Where(e => e.Salary > threshold);
    }

    // 2. Select - project to EmployeeSummary DTO
    public static IEnumerable<EmployeeSummary> ToSummaries(this IEnumerable<Employee> employees)
    {
        return employees.Select(e => new EmployeeSummary(
            e.Id,
            e.Name,
            e.GetType().Name,
            e.Salary));
    }

    // 3. GroupBy - count per department (requires Department context)
    public static IEnumerable<IGrouping<string, Employee>> GroupByDepartment(this IEnumerable<Employee> employees, 
        IDictionary<int, Department> departmentMap)
    {
        return employees
            .GroupBy(e => departmentMap.Values.FirstOrDefault(d => d.Employees.Contains(e))?.Name ?? "Unassigned");
    }

    // Alternative GroupBy without department map - group by role
    public static IEnumerable<IGrouping<string, Employee>> CountByRole(this IEnumerable<Employee> employees)
    {
        return employees
            .GroupBy(e => e.GetType().Name);
    }

    // 4. OrderBy / ThenBy - sort by salary desc, name asc
    public static IOrderedEnumerable<Employee> SortBySalaryDescThenNameAsc(this IEnumerable<Employee> employees)
    {
        return employees
            .OrderByDescending(e => e.Salary)
            .ThenBy(e => e.Name);
    }

    // 5. Join - employees with department names
    public static IEnumerable<EmployeeWithDepartment> JoinWithDepartments(
        this IEnumerable<Employee> employees,
        IEnumerable<Department> departments)
    {
        return employees
            .Join(
                departments,
                e => departments.FirstOrDefault(d => d.Employees.Contains(e))?.Id ?? -1,
                d => d.Id,
                (e, d) => new EmployeeWithDepartment(e.Id, e.Name, d.Name, e.Salary));
    }

    // 6. Any / All - check conditions
    public static bool HasAnyHighEarner(this IEnumerable<Employee> employees, decimal threshold)
    {
        return employees.Any(e => e.Salary > threshold);
    }

    public static bool AllHavePositiveSalary(this IEnumerable<Employee> employees)
    {
        return employees.All(e => e.Salary > 0);
    }

    // 7. Sum / Average / Max - salary aggregates
    public static decimal TotalSalary(this IEnumerable<Employee> employees)
    {
        return employees.Sum(e => e.Salary);
    }

    public static decimal AverageSalary(this IEnumerable<Employee> employees)
    {
        if (!employees.Any()) throw new InvalidOperationException("Sequence contains no elements");
        return employees.Average(e => e.Salary);
    }

    public static Employee? HighestPaid(this IEnumerable<Employee> employees)
    {
        if (!employees.Any()) return null;
        return employees.OrderByDescending(e => e.Salary).First();
    }

    public static decimal MaxSalary(this IEnumerable<Employee> employees)
    {
        if (!employees.Any()) throw new InvalidOperationException("Sequence contains no elements");
        return employees.Max(e => e.Salary);
    }

    // 8. FirstOrDefault with null handling (search by ID)
    public static Employee? FindByIdOrDefault(this IEnumerable<Employee> employees, int id)
    {
        return employees.FirstOrDefault(e => e.Id == id);
    }

    // Bonus: Distinct, Take, Chunk
    public static IEnumerable<string> GetDistinctRoles(this IEnumerable<Employee> employees)
    {
        return employees
            .Select(e => e.GetType().Name)
            .Distinct();
    }

    public static IEnumerable<Employee> GetTopN(this IEnumerable<Employee> employees, int n)
    {
        if (n <= 0) return Enumerable.Empty<Employee>();
        return employees
            .OrderByDescending(e => e.Salary)
            .Take(n);
    }

    public static IEnumerable<Employee[]> ChunkEmployees(this IEnumerable<Employee> employees, int size)
    {
        return employees.Chunk(size);
    }
}

public record EmployeeWithDepartment(int Id, string Name, string DepartmentName, decimal Salary);


