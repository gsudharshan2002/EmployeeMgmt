# EmployeeMgmt
.NET SDK: 10.0.403

## Run Instructions
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the console app
dotnet run --project src/EmployeeMgmt.App
```

## Sample Inputs/Outputs (TODO 2)
**Sample run output highlights:**
1. Seeded 5 employees printed via `StringBuilder` with `PrintAll()`
2. Average salary calculated and displayed (`$78,400.00`)
3. Exception handling demo: duplicate ID and empty name caught with try/catch

## Value vs Reference Types & Nullables
- **Value types:** `int`, `decimal`, `DateTime`, `struct` - stored on stack, copied by value
- **Reference types:** `string`, `class` (`Employee`, `Manager`, `Department`) - stored on heap, referenced by pointer
- **Nullable matters:** `string?` allows null to represent "no value" explicitly. With nullable reference types enabled, the compiler warns when null might cause issues, preventing NullReferenceExceptions.

## .NET Tooling (TODO 6)
- **dotnet restore** - Downloads NuGet packages (e.g., xUnit, test SDK) defined in `.csproj` files
- **dotnet build** - Compiles all projects in the solution, resolving references
- **dotnet test** - Discovers and runs xUnit tests, reports pass/fail
- **dotnet run** - Builds and executes a project
- **.csproj files** - Define target framework, dependencies, references between projects
- **NuGet** - Package manager; xUnit packages are restored automatically
- **Debugger** - Use F5 in VS Code with C# extension to debug with breakpoints

