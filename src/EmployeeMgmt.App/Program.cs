using EmployeeMgmt.Core;
using System.Linq;

namespace EmployeeMgmt.App;

class Program
{
    static async Task Main(string[] args)
    {
        var inMemoryRepo = new InMemoryEmployeeRepository();
        IEmployeeRepository repo = inMemoryRepo;
        
        // Seed with different employee types (polymorphism)
        var emp1 = new Contractor(1, "Alice", 75m, "TechStaff", 12);
        var emp2 = new Contractor(2, "Bob", 80m, "DevPro", 6);
        var manager = new Manager(3, "Carol", 120000m, 15000m);
        var emp3 = new Contractor(4, "David", 65m, "TechStaff", 12);
        var emp4 = new Contractor(5, "Eve", 70m, "DevPro", 9);
        
        manager.AddToTeam(emp1);
        manager.AddToTeam(emp2);
        
        repo.Add(emp1);
        repo.Add(emp2);
        repo.Add(manager);
        repo.Add(emp3);
        repo.Add(emp4);
        
        // Demonstrate polymorphism
        Console.WriteLine("=== Polymorphism Demo (Describe()) ===");
        foreach (var e in repo.GetAll())
        {
            Console.WriteLine(e.Describe());
        }
        
        // Use record DTO
        Console.WriteLine("\n=== EmployeeSummary Records ===");
        var summaries = repo.GetAll()
            .Select(e => new EmployeeSummary(
                e.Id, 
                e.Name, 
                e.GetType().Name, 
                e.Salary))
            .ToList();
        
        foreach (var s in summaries)
        {
            Console.WriteLine($"Summary: {s}");
        }
        
        // Demonstrate encapsulation - Manager's Bonus is private set
        Console.WriteLine($"\n=== Manager Details ===");
        Console.WriteLine($"Manager {manager.Name} has {manager.Team.Count} team members, Bonus: {manager.Bonus:C}");
        
        // Demonstrate Department
        Console.WriteLine("\n=== Department Demo ===");
        var engDept = new Department(1, "Engineering");
        engDept.AddEmployee(emp1);
        engDept.AddEmployee(emp2);
        engDept.AddEmployee(manager);
        Console.WriteLine(engDept.Describe());
        
        // Exception handling
        Console.WriteLine("\n=== Exception Handling ===");
        try
        {
            repo.Add(new Contractor(1, "Duplicate", 75m, "X", 12));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Duplicate] Caught: {ex.Message}");
        }
        
        try
        {
            var newMgr = new Manager(6, "", 50000m, 5000m);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Empty Name] Caught: {ex.Message}");
        }
        
        Console.WriteLine($"\nTotal employees in repo: {repo.Count}");

        Console.WriteLine("\n=== Async + Cancellation Demo ===");
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
}
