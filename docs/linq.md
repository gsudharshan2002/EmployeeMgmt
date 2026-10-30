# LINQ Queries - Input/Output/Complexity

All queries are implemented as extension methods on `IEnumerable<Employee>` in `EmployeeMgmt.Core/EmployeeQueries.cs`. They are pure functions and unit-testable.

| # | Query | Method | Input | Output | Complexity | Notes |
|---|-------|--------|-------|--------|------------|-------|
| 1 | Where | `GetEmployeesEarningAbove(threshold)` | `IEnumerable<Employee>`, `decimal threshold` | `IEnumerable<Employee>` where `Salary > threshold` | O(n) | Filters in single pass. Deferred execution. |
| 2 | Select | `ToSummaries()` | `IEnumerable<Employee>` | `IEnumerable<EmployeeSummary>` | O(n) | Projects each element to record DTO. Deferred. |
| 3a | GroupBy | `GroupByDepartment(departmentMap)` | `IEnumerable<Employee>`, `IDictionary<int, Department>` | `IEnumerable<IGrouping<string, Employee>>` | O(n × m) where m = departments checked; effectively O(n) in typical usage | Groups by department name via lookup. |
| 3b | GroupBy | `CountByRole()` | `IEnumerable<Employee>` | `IEnumerable<IGrouping<string, Employee>>` grouped by `GetType().Name` | O(n) | Groups by role. Count via `g.Count()`. |
| 4 | OrderBy/ThenBy | `SortBySalaryDescThenNameAsc()` | `IEnumerable<Employee>` | `IOrderedEnumerable<Employee>` | O(n log n) | Sorts by Salary desc, then Name asc. Deferred until enumerated. |
| 5 | Join | `JoinWithDepartments(departments)` | `IEnumerable<Employee>`, `IEnumerable<Department>` | `IEnumerable<EmployeeWithDepartment>` | O(n × m) naive Contains; O(n+m) with hash sets | Joins employees to departments. |
| 6a | Any | `HasAnyHighEarner(threshold)` | `IEnumerable<Employee>`, `decimal` | `bool` | O(k) where k = first match found (early termination) | Short-circuits on first match. |
| 6b | All | `AllHavePositiveSalary()` | `IEnumerable<Employee>` | `bool` | O(k) where k = first false found | Short-circuits on first violation. |
| 7a | Sum | `TotalSalary()` | `IEnumerable<Employee>` | `decimal` | O(n) | Single pass aggregation. |
| 7b | Average | `AverageSalary()` | `IEnumerable<Employee>` | `decimal` | O(n) | Throws if empty (like LINQ Average). |
| 7c | Max/Find highest | `HighestPaid()`, `MaxSalary()` | `IEnumerable<Employee>` | `Employee?` / `decimal` | O(n) | Single pass or sort - implemented as sort+First for clarity. |
| 8 | FirstOrDefault + null handling | `FindByIdOrDefault(id)` | `IEnumerable<Employee>`, `int` | `Employee?` | O(k) where k = position of match | Returns null if not found; caller must null-check. |
| Bonus 1 | Distinct | `GetDistinctRoles()` | `IEnumerable<Employee>` | `IEnumerable<string>` | O(n) average | Uses default equality comparer. |
| Bonus 2 | Take | `GetTopN(n)` | `IEnumerable<Employee>`, `int` | `IEnumerable<Employee>` | O(n log n) due to OrderBy | Takes first n after sorting. |
| Bonus 3 | Chunk | `ChunkEmployees(size)` | `IEnumerable<Employee>`, `int` | `IEnumerable<Employee[]>` | O(n) | Splits into arrays of size chunks. |

## Key Concepts

- **IEnumerable vs IQueryable**: `IEnumerable<T>` is in-memory (LINQ to Objects), executes with delegates. `IQueryable<T>` is provider-based (LINQ to SQL/EF), translates expressions to query language.
- **Deferred execution**: Most LINQ operators return `IEnumerable<T>` without enumerating. Execution happens when iterated (foreach, ToList, Count, etc.). `ToList()`, `Count()`, `Sum()`, `Any()`, `All()`, `FirstOrDefault()` force immediate execution.
- **List vs Dictionary**: `List<T>` is ordered by index, O(1) access by index, O(n) search. `Dictionary<TKey,TValue>` is key-based, O(1) average lookup/insert/delete by key.
