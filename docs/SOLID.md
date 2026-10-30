# SOLID Principles Explanation

## S - Single Responsibility Principle (SRP)
Each class should have one reason to change. In our design:
- `Employee` hierarchy handles employee identity and behavior (polymorphic `Describe()`).
- `Department` is responsible only for grouping employees.
- `InMemoryEmployeeRepository` is responsible only for data persistence/retrieval (CRUD operations).
- `EmployeeSummary` (record) is responsible only for data transfer representation.

## O - Open/Closed Principle (OCP)
Entities should be open for extension but closed for modification. `Employee` is `abstract` with virtual `Describe()`. We added `Manager` and `Contractor` by extending `Employee` without modifying the base class. New employee types can be added without changing existing code.

## L - Liskov Substitution Principle (LSP)
Subtypes must be substitutable for their base types. Both `Manager` and `Contractor` inherit from `Employee` and can be used anywhere `Employee` is expected. `repo.Add(manager)` and `repo.Add(contractor)` work because they are both `Employee`. The polymorphic `Describe()` calls work correctly through base type references.

## I - Interface Segregation Principle (ISP)
Clients should not be forced to depend on interfaces they don't use. `IEmployeeRepository` is focused and cohesive - it only contains methods needed for repository operations (`Add`, `GetById`, `GetAll`, `Remove`, `Count`). No fat interfaces forcing unnecessary implementations.

## D - Dependency Inversion Principle (DIP)
High-level modules should not depend on low-level modules; both should depend on abstractions. In `Program.cs`, we depend on `IEmployeeRepository` (abstraction), not `InMemoryEmployeeRepository` (concrete implementation). We can swap implementations without changing the consuming code. The App depends on the interface from Core, following the dependency rule.
