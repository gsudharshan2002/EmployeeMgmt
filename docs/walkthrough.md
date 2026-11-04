# Gate 1 Walkthrough

## 1. Class Demo: InMemoryEmployeeRepository (Design + SOLID)

**File:** `src/EmployeeMgmt.Core/InMemoryEmployeeRepository.cs`

**Design:**
- Implements `IEmployeeRepository` interface - depends on abstraction
- Uses `private readonly List<Employee> _employees = new()` for in-memory storage
- Enforces business rules: no duplicate IDs (`_employees.Any(x => x.Id == e.Id)`), validates null
- Provides CRUD: `Add`, `GetById`, `GetAll`, `Remove`, plus `Count`
- Has async method `LoadEmployeesAsync` with `CancellationToken`

**SOLID Principles Applied:**
- **SRP:** Repository only handles data access - Employee domain objects handle their own behavior
- **DIP:** App depends on `IEmployeeRepository` interface, not concrete `InMemoryEmployeeRepository`
- **LSP:** Any implementation of `IEmployeeRepository` can substitute this one
- **ISP:** Interface is focused, only repository methods
- **OCP:** Can create new repository implementations (e.g., DbEmployeeRepository) without modifying consumers

**Code Walkthrough:**
```csharp
IEmployeeRepository repo = new InMemoryEmployeeRepository(); // DIP in action
repo.Add(emp); // Encapsulated business logic
```

## 2. LINQ Query Demo: GroupBy Count Per Department (Output + Complexity)

**Query:** `CountByRole()` in `src/EmployeeMgmt.Core/EmployeeQueries.cs`

```csharp
public static IEnumerable<IGrouping<string, Employee>> CountByRole(this IEnumerable<Employee> employees)
{
    return employees
        .GroupBy(e => e.GetType().Name);
}
```

**Input:** List of mixed Employee types (Contractors and Managers)

**Output Example:**
```
Contractor: 4 employees
Manager: 1 employee
```

**Complexity:** O(n) average - single pass to group elements by key. Deferred execution until enumerated.

**Usage:**
```csharp
var grouped = employees.CountByRole();
foreach(var group in grouped) {
    Console.WriteLine($"{group.Key}: {group.Count()} employees");
}
```

**Note:** `GroupBy` is deferred - execution happens on enumeration.

## 3. Async Operation Demo: LoadEmployeesAsync (Cancellation + Pitfalls)

**File:** `src/EmployeeMgmt.Core/InMemoryEmployeeRepository.cs`

```csharp
public async Task<IReadOnlyList<Employee>> LoadEmployeesAsync(CancellationToken ct = default)
{
    await Task.Delay(500, ct); // simulate I/O
    ct.ThrowIfCancellationRequested();
    return _employees.ToList();
}
```

**Usage in App:** `src/EmployeeMgmt.App/Program.cs`
```csharp
try {
    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
    var loaded = await inMemoryRepo.LoadEmployeesAsync(cts.Token);
    Console.WriteLine($"Loaded {loaded.Count} employees asynchronously.");
} catch (OperationCanceledException) {
    Console.WriteLine("Operation was cancelled due to timeout.");
}
```

**Key Points:**
- **Task vs Thread:** `async/await` doesn't create a new thread. `Task.Delay` uses timers. It's about freeing up threads while waiting for I/O.
- **CancellationToken:** Passed through to `Task.Delay` which respects it. We also call `ThrowIfCancellationRequested()`.
- **Cancellation:** When cancelled, `Task.Delay` throws `TaskCanceledException` (derived from `OperationCanceledException`).
- **Pitfalls:**
  - Don't block with `.Result` or `.Wait()` - causes deadlocks
  - Always pass and check `CancellationToken`
  - Async doesn't make CPU-bound work faster
  - Exceptions from async methods are wrapped in the Task

## Knowledge Check Answers

### OOP
- **Encapsulation:** Hiding data - `Employee.Id` has private setter, `Manager.Bonus` private setter, accessed through controlled methods
- **Inheritance:** `Manager` and `Contractor` inherit from `Employee` base class
- **Polymorphism:** Override `Describe()` - called on base `Employee` reference but executes derived class implementation
- **Abstraction:** `Employee` is abstract, can't instantiate directly. `IEmployeeRepository` abstracts data access.

### LINQ
- **Where:** Filters elements
- **Select:** Projects/transforms elements
- **GroupBy:** Groups by key
- **OrderBy:** Sorts
- **Deferred execution:** Query not executed until enumerated (foreach, ToList, Count)

### Collections
- **Array:** Fixed size, fastest for index access, value/reference depending on type
- **List<T>:** Dynamic size, ordered, O(1) end access, O(n) search
- **Dictionary<K,V>:** Key-value pairs, O(1) average lookup by key, unordered

### Async/Await
- **Task:** Represents async operation
- **CancellationToken:** Cooperative cancellation mechanism
- **async ≠ multithreaded:** Uses async I/O, doesn't necessarily spawn threads

### SOLID (DIP Example)
In our code: `Program` depends on `IEmployeeRepository` (abstraction) not `InMemoryEmployeeRepository` (concrete). Can swap implementations without changing App code. Both depend on abstraction.

### Testing
- **Good test:** Fast, isolated, repeatable, self-validating, timely (FIRST)
- **AAA:** Arrange (setup), Act (execute), Assert (verify)
- **Normal:** Valid inputs produce expected results
- **Boundary:** Edge cases (empty, single item, min/max)
- **Invalid:** Bad inputs throw expected exceptions
