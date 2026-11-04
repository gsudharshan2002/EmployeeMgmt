# EmployeeMgmt - Complete TODO Details (1-10)

**Repository:** https://github.com/gsudharshan2002/EmployeeMgmt  
**SDK:** .NET 10.0.403  
**Domain:** Employee & Department Management  
**Last Commit:** b9c0dc2

---

## TODO 1 — Lock In Foundations (Domain + SDK + Repo) ✅

**When:** Before Day 1  
**Commit:** 282dba2

### What We Did
- Confirmed SDK version: `10.0.403`
- Created solution structure with Console, Class Library, and xUnit Test projects
- Initialized Git repo with `.gitignore` and `README.md`
- Added remote origin and pushed

### Files Created/Modified
| File | Purpose |
|------|---------|
| `EmployeeMgmt.slnx` | Solution file containing all projects |
| `src/EmployeeMgmt.App/EmployeeMgmt.App.csproj` | Console app project (references Core) |
| `src/EmployeeMgmt.Core/EmployeeMgmt.Core.csproj` | Class library project |
| `tests/EmployeeMgmt.Tests/EmployeeMgmt.Tests.csproj` | xUnit test project (references Core, has xUnit packages) |
| `.gitignore` | .NET gitignore |
| `README.md` | Project documentation with SDK version |

### Concepts Used
- Solution structure, Project references, Git basics

---

## TODO 2 — Day 1: C# Fundamentals Console App ✅

**Commit:** 51e826f

### Goal
Move programming concepts into C# syntax and type semantics using methods + collections.

### Files Created/Modified
| File | Action | Details |
|------|--------|---------|
| `src/EmployeeMgmt.App/Program.cs` | **Modified** | Complete rewrite from "Hello World" to employee management console app |

### Code Structure in Program.cs

```csharp
public sealed class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Department { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
}

public sealed class EmployeeRepository
{
    private readonly List<Employee> _employees = new();
    
    public EmployeeRepository() { Seed(); }
    
    private void Seed() { /* Seeds 5 employees */ }
    
    public void AddEmployee(Employee emp) { /* Validates name & duplicate ID */ }
    public Employee? FindById(int id) { /* Searches, returns null if not found */ }
    public string PrintAll() { /* Uses StringBuilder */ }
    public decimal AverageSalary() { /* Calculates average */ }
}

class Program
{
    static void Main(string[] args) { /* Demo all functionality with try/catch */ }
}
```

### Features Implemented
- ✅ `Employee` class (reference type)
- ✅ `List<Employee>` seeded with 5+ records
- ✅ Methods: `AddEmployee`, `FindById`, `PrintAll`, `AverageSalary`
- ✅ `string`, `StringBuilder`, nullable types (`string?`)
- ✅ Arrays used where sensible
- ✅ 2 exceptions handled: duplicate ID (`InvalidOperationException`), empty name (`ArgumentException`) with try/catch

### Concepts Used
| Concept | Explanation | Example in Code |
|---------|-------------|-----------------|
| **Value vs Reference Types** | Value types (int, decimal, struct) stored on stack, copied by value. Reference types (class, string) stored on heap, copied by reference. | `Employee` is a class (reference), `int`/`decimal` are value types |
| **string vs StringBuilder** | `string` is immutable. `StringBuilder` is mutable, efficient for multiple concatenations. | `PrintAll()` uses `StringBuilder` to build output |
| **Nullable Types** | `string?` allows null values. Helps prevent null reference exceptions with nullable reference types enabled. | `public string? Department { get; set; }`, `FindById` returns `Employee?` |
| **Collections** | `List<T>` is dynamic array, grows as needed. | `private readonly List<Employee> _employees = new()` |
| **Exception Handling** | Use try/catch to handle expected errors gracefully. | Catches duplicate ID and empty name exceptions |

### Interview Q&A
**Q: What's the difference between value and reference types?**  
A: Value types hold data directly and are copied on assignment. Reference types hold a reference to heap data. Classes are reference types, structs/int are value types.

**Q: Why use StringBuilder over string concatenation?**  
A: String is immutable - each concatenation creates new string objects. StringBuilder is mutable and efficient for building strings in loops.

**Q: What are nullable reference types?**  
A: In C# 8+, `?` indicates a reference type can be null. With nullable enabled, compiler warns about potential null dereferences, helping prevent NullReferenceExceptions.

---

## TODO 3 — Day 2: OOP Domain Models + Interfaces ✅

**Commit:** b9db1da

### Goal
Idiomatic enterprise-style object modeling with OOP principles.

### Files Created in `src/EmployeeMgmt.Core/`
| File | Type | Details |
|------|------|---------|
| `Employee.cs` | Abstract Class | Base class with encapsulation, virtual method |
| `Manager.cs` | Sealed Class (Inheritance) | Inherits Employee, has Team composition, Bonus |
| `Contractor.cs` | Sealed Class (Inheritance) | Inherits Employee, has Agency, ContractMonths |
| `Department.cs` | Sealed Class | Aggregates employees, encapsulated collection |
| `IEmployeeRepository.cs` | Interface | Repository contract |
| `InMemoryEmployeeRepository.cs` | Sealed Class (Implements Interface) | In-memory implementation using List |
| `EmployeeSummary.cs` | Record (DTO) | Immutable data transfer object |

### Files Modified
| File | Changes |
|------|---------|
| `src/EmployeeMgmt.App/Program.cs` | Now uses `IEmployeeRepository` (DIP), demonstrates polymorphism |
| `src/EmployeeMgmt.Core/Class1.cs` | Deleted (replaced) |

### Files Created in `docs/`
| File | Purpose |
|------|---------|
| `docs/class-diagram.mmd` | Mermaid UML class diagram |
| `docs/SOLID.md` | SOLID principles explained with examples |

### Key Implementations

**Encapsulation (private setters):**
```csharp
public abstract class Employee
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Salary { get; protected set; }
    // ...
}
```

**Inheritance + Polymorphism:**
```csharp
public override string Describe() => $"Manager {Id}: {Name}, TeamSize: {Team.Count}, TotalComp: {(Salary + Bonus):C}";
```

**Interface + Implementation (DIP):**
```csharp
public interface IEmployeeRepository
{
    void Add(Employee e);
    Employee? GetById(int id);
    IEnumerable<Employee> GetAll();
    void Remove(int id);
    int Count { get; }
}

public sealed class InMemoryEmployeeRepository : IEmployeeRepository { ... }
```

**Record DTO:**
```csharp
public record EmployeeSummary(int Id, string Name, string Role, decimal Salary);
```

### SOLID Principles Applied

| Principle | Definition | Our Implementation |
|-----------|------------|-------------------|
| **S - Single Responsibility** | Class should have one reason to change | Repository handles data access only. Employee handles employee behavior. Department manages groups. |
| **O - Open/Closed** | Open for extension, closed for modification | `Employee` is abstract with virtual `Describe()`. Add new types (Manager, Contractor) without modifying base class. |
| **L - Liskov Substitution** | Subtypes must be substitutable for base types | `Manager` and `Contractor` can be used anywhere `Employee` is expected. |
| **I - Interface Segregation** | Don't force clients to depend on unused methods | `IEmployeeRepository` is focused with only needed methods. |
| **D - Dependency Inversion** | Depend on abstractions, not concretions | `Program` uses `IEmployeeRepository`, not `InMemoryEmployeeRepository`. | 

### Concepts Used
- Classes, Abstract Classes, Sealed Classes, Interfaces
- Inheritance, Polymorphism (virtual/override), Abstraction
- Encapsulation (private/protected setters)
- Composition (Manager has Team of Employees), Aggregation (Department has Employees)
- Records (immutable DTOs)
- SOLID Principles

### Interview Q&A
**Q: Interface vs Abstract Class?**  
A: Interface defines contract (can implement multiple). Abstract class can have implementation, fields, constructors. Use interface for capability, abstract class for shared base behavior.

**Q: Composition vs Inheritance?**  
A: Inheritance is "is-a" relationship (Manager IS-A Employee). Composition is "has-a" relationship (Manager HAS-A Team). Prefer composition over inheritance for flexibility.

**Q: What is Dependency Inversion?**  
A: High-level modules shouldn't depend on low-level modules. Both depend on abstractions. In our code, App depends on `IEmployeeRepository` interface, not concrete class.

---

## TODO 4 — Day 3: LINQ + Collections ✅

**Commit:** b4dfd38

### Goal
Fluent data transformation over in-memory records.

### Files Created
| File | Details |
|------|---------|
| `src/EmployeeMgmt.Core/EmployeeQueries.cs` | 8+ LINQ queries as extension methods (pure, unit-testable) |
| `docs/linq.md` | Per-query table with input/output/complexity + concepts |

### LINQ Queries Implemented

| # | Operator | Method | Purpose | Complexity |
|---|----------|--------|---------|------------|
| 1 | **Where** | `GetEmployeesEarningAbove(threshold)` | Filter employees earning above X | O(n) |
| 2 | **Select** | `ToSummaries()` | Project to `EmployeeSummary` DTO | O(n) |
| 3 | **GroupBy** | `GroupByDepartment()`, `CountByRole()` | Group by department/role | O(n) |
| 4 | **OrderBy/ThenBy** | `SortBySalaryDescThenNameAsc()` | Sort by salary desc, name asc | O(n log n) |
| 5 | **Join** | `JoinWithDepartments()` | Join employees with department names | O(n*m) / O(n+m) optimized |
| 6 | **Any/All** | `HasAnyHighEarner()`, `AllHavePositiveSalary()` | Check conditions (short-circuit) | O(k) early termination |
| 7 | **Sum/Average/Max** | `TotalSalary()`, `AverageSalary()`, `HighestPaid()`, `MaxSalary()` | Aggregates | O(n) |
| 8 | **FirstOrDefault** | `FindByIdOrDefault(id)` | Search by ID with null handling | O(k) |
| **Bonus** | **Distinct/Take/Chunk** | `GetDistinctRoles()`, `GetTopN()`, `ChunkEmployees()` | Additional operators | Varies |

### Key Concepts

**IEnumerable vs IQueryable:**
- `IEnumerable<T>`: In-memory (LINQ to Objects), uses delegates, executes client-side
- `IQueryable<T>`: Provider-based (LINQ to SQL/EF), translates expressions to SQL/query language

**Deferred Execution:**
Most LINQ methods return `IEnumerable<T>` without enumerating. Execution happens when iterated (`foreach`, `ToList()`, `Count()`, `Sum()`, `FirstOrDefault()` force immediate execution).

**List vs Dictionary:**
- `List<T>`: Ordered by index, O(1) index access, O(n) search
- `Dictionary<K,V>`: Key-value pairs, O(1) average lookup/insert by key, unordered
- `Array`: Fixed size, fastest for known size

### Interview Q&A
**Q: What is deferred execution in LINQ?**  
A: LINQ queries aren't executed when defined, only when enumerated. This allows building complex queries efficiently.

**Q: When to use List vs Dictionary?**  
A: Use List when order matters or accessing by index. Use Dictionary when looking up by key frequently.

**Q: What's the difference between Any() and Count() > 0?**  
A: `Any()` short-circuits on first match (more efficient). `Count()` may enumerate entire collection.

---

## TODO 5 — Day 4a: xUnit Test Project ✅

**Commit:** 141163e

### Goal
Unit testing with AAA pattern, Fact and Theory.

### Files Created
| File | Details |
|------|---------|
| `tests/EmployeeMgmt.Tests/EmployeeQueriesTests.cs` | 10 tests with AAA pattern |
| `tests/EmployeeMgmt.Tests/UnitTest1.cs` | Deleted (placeholder removed) |

### Test Coverage
- [Fact] tests for single scenarios
- [Theory] with [InlineData] for parameterized tests
- Covers: filtering, projection, sorting, GetById (found + not found), aggregates, empty list, Any/All

### Example Test (AAA Pattern)
```csharp
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
```

### Concepts Used
- xUnit ([Fact], [Theory], [InlineData])
- AAA Pattern (Arrange-Act-Assert)
- Assertions (Assert.Equal, Assert.NotNull, Assert.Null, Assert.Single, Assert.Empty)
- Test project references

### Interview Q&A
**Q: What makes a good unit test?**  
A: Fast, isolated (no external dependencies), repeatable, self-validating, thorough (FIRST principles). Tests one thing, clear naming.

**Q: Explain AAA pattern?**  
A: Arrange - set up test data/dependencies. Act - execute the method under test. Assert - verify expected outcome.

**Q: Fact vs Theory in xUnit?**  
A: [Fact] runs once with no parameters. [Theory] runs multiple times with different data from [InlineData], [MemberData], etc.

---

## TODO 6 — Day 4b: Async + Cancellation + .NET Tooling ✅

**Commit:** 065b0a4

### Goal
Add async behavior with cancellation and document tooling.

### Files Modified
| File | Changes |
|------|---------|
| `src/EmployeeMgmt.Core/InMemoryEmployeeRepository.cs` | Added `LoadEmployeesAsync` with `CancellationToken` |
| `src/EmployeeMgmt.App/Program.cs` | Changed to async Main, added cancellation demo |
| `README.md` | Added run instructions, samples, tooling section |
| `tests/EmployeeMgmt.Tests/AsyncCancellationTests.cs` | Created (new) |

### Implementation

**Async Method:**
```csharp
public async Task<IReadOnlyList<Employee>> LoadEmployeesAsync(CancellationToken ct = default)
{
    await Task.Delay(500, ct);
    ct.ThrowIfCancellationRequested();
    return _employees.ToList();
}
```

**App Usage with Cancellation:**
```csharp
static async Task Main(string[] args)
{
    var inMemoryRepo = new InMemoryEmployeeRepository();
    // ...
    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
        var loaded = await inMemoryRepo.LoadEmployeesAsync(cts.Token);
        Console.WriteLine($"Loaded {loaded.Count} employees asynchronously.");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Operation was cancelled due to timeout.");
    }
}
```

**Cancellation Test:**
```csharp
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
    await Assert.ThrowsAsync<TaskCanceledException>(() => repo.LoadEmployeesAsync(cts.Token));
}
```

### .NET Tooling Explained
| Command | Purpose |
|---------|---------|
| `dotnet restore` | Downloads NuGet packages from dependencies |
| `dotnet build` | Compiles all projects, resolves references |
| `dotnet test` | Discovers and runs tests, reports results |
| `dotnet run` | Builds and runs the project |
| `.csproj` | Project file defining target framework, packages, references |
| **NuGet** | Package manager (xUnit packages restored automatically) |
| **Debugger (F5)** | Debug with breakpoints in VS Code with C# extension |

### Concepts Used
- async/await, Task<T>, CancellationToken, CancellationTokenSource
- OperationCanceledException/TaskCanceledException
- Async Main
- `using` with disposables

### Interview Q&A
**Q: Task vs Thread - what's the difference?**  
A: `Task` represents an async operation; it doesn't necessarily create a new thread. `async/await` uses compiler state machines. I/O operations (like Task.Delay) don't block threads - they're asynchronous. Threads are for CPU-bound parallel work.

**Q: Why does async != multithreaded?**  
A: Async is about asynchronicity (waiting without blocking). Multithreading is about parallelism (doing multiple things simultaneously). `Task.Delay` uses a timer, no thread is blocked/consumed during wait.

**Q: What are async pitfalls?**  
A: Avoid `.Result`/`.Wait()` (deadlocks in UI/ASP.NET), don't use `async void` except event handlers, always propagate CancellationToken, be careful with exception handling.

---

## TODO 7 — Day 5a: Refactor + Full Test Coverage ✅

**Commit:** ad0e11d

### Goal
Refactor for clarity/testability, cover normal/boundary/invalid inputs.

### Files Created
| File | Details |
|------|---------|
| `tests/EmployeeMgmt.Tests/ComprehensiveTests.cs` | Tests for normal, boundary, invalid cases |

### Test Categories

**Normal Cases:**
- `GetTopN_NormalData_ReturnsCorrectCount()` - Valid data
- `ToSummaries_ProjectsToDto_ReturnsEmployeeSummaryRecords()` - Projection

**Boundary Cases:**
- `GetTopN_BoundarySingleItem_ReturnsOne()` - Single item
- `GetTopN_BoundaryEmptyList_ReturnsEmpty()` - Empty list
- `AverageSalary_BoundaryEmptyList_ThrowsException()` - Empty aggregation
- `HighestPaid_BoundaryEmptyList_ReturnsNull()` - Empty returns null
- `MaxSalary_BoundaryEmptyList_ThrowsException()` - Empty throws

**Invalid Cases:**
- `InMemoryRepository_InvalidDuplicateId_ThrowsException()` - Duplicate ID
- `InMemoryRepository_InvalidNullInput_ThrowsArgumentNullException()` - Null input
- `Employee_InvalidEmptyName_ThrowsArgumentException()` - Empty name
- `Department_InvalidEmptyName_ThrowsArgumentException()` - Empty department name

**Coverage Command:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

**Results:** 23 tests passing, coverage report generated

### Interview Q&A
**Q: What's the difference between boundary and invalid input testing?**  
A: Boundary tests edge values (empty, min/max, single element). Invalid tests bad data that should be rejected (null, duplicates, empty required fields) - should throw exceptions or be handled.

---

## TODO 8 — Day 5b: Inject & Fix a Defect (Regression Test) ✅

**Commits:** e2f2547 (defect injected), 296b430 (fixed)

### Goal
Prove debugging + regression discipline.

### Defect Details
**What:** Off-by-one error in `GetTopN` method  
**Where:** `src/EmployeeMgmt.Core/EmployeeQueries.cs`  
**How it failed:** Used `Take(n - 1)` instead of `Take(n)`, returning one less item than requested

### Sequence

**Step 1 - Inject Defect (e2f2547):**
```csharp
public static IEnumerable<Employee> GetTopN(this IEnumerable<Employee> employees, int n)
{
    return employees
        .OrderByDescending(e => e.Salary)
        .Take(n > 0 ? n - 1 : 0); // DEFECT: off-by-one
}
```

**Step 2 - Write Failing Regression Test:**
```csharp
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
    
    // Assert - Regression test
    Assert.Equal(2, top2.Count); // Failed - got 1
    Assert.Equal("A", top2[0].Name);
    Assert.Equal("B", top2[1].Name);
}
```
**Result:** 3 tests failed (including 2 existing tests)

**Step 3 - Fix Defect (296b430):**
```csharp
public static IEnumerable<Employee> GetTopN(this IEnumerable<Employee> employees, int n)
{
    if (n <= 0) return Enumerable.Empty<Employee>();
    return employees
        .OrderByDescending(e => e.Salary)
        .Take(n); // FIXED
}
```
**Result:** All 24 tests passing

### Concepts Used
- Debugging, Regression testing, Off-by-one errors
- Git commit history showing defect → test → fix

### Interview Q&A
**Q: What is a regression test?**  
A: A test that ensures a bug fix stays fixed. We reproduce the bug with a failing test, fix it, and the test passes - preventing reintroduction.

---

## TODO 9 — Gate 1 Prep: Knowledge Check + Code Walkthrough ✅

**Commit:** 9685a63

### Files Created
| File | Purpose |
|------|---------|
| `docs/walkthrough.md` | 3 demos + complete knowledge check |

### 3 Demos Prepared

**Demo 1: InMemoryEmployeeRepository (Design + SOLID)**
- Shows interface implementation, encapsulation, business rules
- Explains all 5 SOLID principles with code references

**Demo 2: GroupBy CountByRole (Output + Complexity)**
- LINQ GroupBy query
- Output example, O(n) complexity
- Deferred execution explained

**Demo 3: LoadEmployeesAsync (Cancellation + Pitfalls)**
- Async/await with CancellationToken
- Task.Delay with cancellation
- Task vs Thread distinction, common pitfalls

### Full Knowledge Check Covered
- OOP (encapsulation, inheritance, polymorphism, abstraction)
- LINQ (Where, Select, GroupBy, OrderBy, deferred execution)
- Collections (List vs Dictionary vs array)
- Async/await (Task, CancellationToken, async ≠ multithreaded)
- SOLID (with concrete example)
- Testing (good tests, AAA, boundary vs invalid)

---

## TODO 10 — LeetCode Log + Final Submission Package ✅

**Commit:** b9c0dc2

### Files Created
| File | Purpose |
|------|---------|
| `docs/leetcode.md` | Complete LeetCode log |

### LeetCode Problems

| Day | Problem | Approach | Time | Space | Lesson |
|---|---------|----------|------|-------left |
| 1 | Concatenation of Array | Array copy twice | O(n) | O(n) | Pre-allocate array |
| 2 | Number of Steps to Reduce to Zero | Even/odd division/subtraction | O(log n) | O(1) | Iterative approach |
| 3 | Contains Duplicate | HashSet lookup | O(n) | O(n) | HashSet for fast lookup |
| 4 | Best Time to Buy and Sell Stock | Track min price, max profit | O(n) | O(1) | Single pass suffices |
| 5 | Valid Anagram | Character frequency count | O(n) | O(1) | Frequency counting efficient |

### Final Checklist Status
- ✅ Git repo with all code, commits per day
- ✅ README.md: run instructions, SDK version, 3 sample I/O, domain choice
- ✅ docs/: UML diagram, SOLID explanation, LINQ table, walkthrough, LeetCode log
- ✅ dotnet build + dotnet test logs (all green - 24/24)
- ✅ Async + cancellation implementation + test
- ✅ Regression test for injected defect
- ✅ Knowledge check practiced

---

## Complete Concepts Summary

### C# Fundamentals
- Types (value vs reference), nullable types, string vs StringBuilder
- Collections (List, arrays), loops, methods
- Exception handling (try/catch/throw)

### OOP
- Classes, inheritance, polymorphism, abstraction, encapsulation
- Interfaces, abstract classes, sealed classes
- Records, composition vs aggregation
- SOLID principles

### LINQ & Functional
- LINQ operators (Where, Select, GroupBy, Join, OrderBy, Any/All, aggregates)
- Extension methods, lambda expressions
- Deferred execution
- Pure functions

### Testing
- xUnit, [Fact]/[Theory], AAA pattern
- Normal/boundary/invalid testing
- Regression testing
- Code coverage

### Async & Tooling
- async/await, Task, CancellationToken
- dotnet CLI (restore/build/test/run)
- NuGet, csproj files, debugging
- Git workflow

### All Commits
```
282dba2 TODO 1: Foundation
51e826f TODO 2: C# Fundamentals
b9db1da TODO 3: OOP + Interfaces + SOLID
b4dfd38 TODO 4: LINQ
141163e TODO 5: xUnit Tests
065b0a4 TODO 6: Async + Cancellation + Tooling
ad0e11d TODO 7: Refactor + Coverage
e2f2547 TODO 8: Inject defect
296b430 TODO 8: Fix defect
9685a63 TODO 9: Walkthrough
b9c0dc2 TODO 10: LeetCode + Final Package
```

**Total Tests:** 24 passing  
**Build Status:** Success  
**All TODOs:** Complete ✅
