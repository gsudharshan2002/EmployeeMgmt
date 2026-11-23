# EmployeeMgmt — .NET Web API (Week 2)

An ASP.NET Core REST API for employee and department management, built in three layers with EF Core/SQLite, validation, consistent error responses, and JWT-protected delete operations. The original console app remains available as a Week 1 learning example.

## Requirements and run

- .NET SDK 10.0 or later
- SQLite (the default provider is bundled by the EF Core SQLite package)

Restore and install the EF migration tool:

```bash
dotnet restore
dotnet tool install --global dotnet-ef --version 10.0.15
```

Set local-only authentication settings in the shell that will run the API. Do not commit credentials or signing keys:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export Authentication__SigningKey="$(openssl rand -base64 48)"
export Authentication__AdminUsername=admin
export Authentication__AdminPassword='replace-with-a-local-admin-password'
export Authentication__UserUsername=employee
export Authentication__UserPassword='replace-with-a-local-user-password'
```

Apply the checked-in migration, then start the API:

```bash
dotnet ef database update \
  --project src/EmployeeMgmt.Infrastructure/EmployeeMgmt.Infrastructure.csproj \
  --startup-project src/EmployeeMgmt.Api/EmployeeMgmt.Api.csproj

dotnet run --project src/EmployeeMgmt.Api/EmployeeMgmt.Api.csproj --urls http://localhost:5080
```

The default SQLite file is `src/EmployeeMgmt.Api/employee-management.db`. Override the `ConnectionStrings__EmployeeDb` environment variable to use another database. Swagger UI is available at `http://localhost:5080/swagger` in Development. Example success and failure requests are in [`docs/employee-api.http`](docs/employee-api.http).

## API

| Method | Route | Behavior |
| --- | --- | --- |
| GET | `/api/employees` | Search, filter by department, sort by `name`, `salary`, or `id`, and paginate |
| GET | `/api/employees/{id}` | Return an employee or 404 |
| POST | `/api/employees` | Create an employee; returns 201 and a `Location` |
| PUT | `/api/employees/{id}` | Replace editable employee fields; returns 200 or 404 |
| DELETE | `/api/employees/{id}` | Admin-only; returns 204, 401, 403, or 404 |
| GET/POST | `/api/departments` | List or create departments |
| GET/PUT/DELETE | `/api/departments/{id}` | Read, update, or admin-only delete |
| POST | `/api/auth/token` | Exchange configured local credentials for a 30-minute JWT |

Employee listing accepts `search`, `departmentId`, `sortBy`, `descending`, `page`, and `pageSize`; page size is capped by `Pagination:MaxPageSize` (100 by default). Employee and department names and employee salary are validated. Invalid requests return ProblemDetails; missing resources return 404, invalid query/business input returns 400, and conflicting operations (such as deleting a non-empty department) return 409.

### Authorization demo

Configure separate `Authentication__Admin*` and `Authentication__User*` credentials as above. Request a token from `/api/auth/token`, then try an employee DELETE:

- No token: **401 Unauthorized** (not authenticated).
- Employee-role token: **403 Forbidden** (authenticated, but not authorized).
- Admin-role token: **204 No Content** when the employee exists.

The signing key, credentials, issuer, and audience are supplied through configuration; no credentials are checked into this repository. Development generates a process-local signing key if one is not configured. Configure a stable secret through a secret manager outside Development. This training token issuer supports two configured demo identities only; it is not a production identity provider and has no refresh or revocation flow.

## Architecture and implementation notes

See [`docs/architecture.mmd`](docs/architecture.mmd) for project dependencies and [`docs/request-pipeline.mmd`](docs/request-pipeline.mmd) for middleware order.

- **API:** controller routing, HTTP semantics, OpenAPI/Swagger, JWT authentication and authorization.
- **Application:** employee/department services, request contracts, business validation, pagination options, and store abstractions.
- **Infrastructure:** scoped EF Core `DbContext`, SQLite mappings, migrations, relationship loading, and persistence queries.
- **Core:** Employee and Department entities; the earlier polymorphism/console examples remain in the console app.

The `DbContext`, stores, and application services are scoped to a request. The stateless token issuer and bound options use singleton options lifetime. Conventional middleware is activated by ASP.NET Core. No transient application dependency is needed by this feature set.

Employee-to-department is an optional many-to-one relationship; deleting a department with employees is restricted. Employee list queries use `AsNoTracking`, eager-load the department, and apply filtering/count/order/skip/take in SQLite. Detail/update paths track entities where changes are saved. To inspect generated SQL, set EF Core logging to `Information` or capture the SQL in a debugger.

## Tests

```bash
dotnet build EmployeeMgmt.slnx
dotnet test EmployeeMgmt.slnx
```

Tests cover the existing core behavior, service validation, SQLite relationship/filter/sort/page queries, API validation and status codes, and JWT login plus unauthenticated/forbidden/authorized delete outcomes. The integration tests apply the checked-in migration to an isolated temporary SQLite database.

## Known limitations

- The API exposes generic Employee records; the Week 1 `Manager` and `Contractor` console examples are not part of EF persistence.
- SQLite is the local training database; select and configure a production database provider for deployment.
- The configured demo identities are not a substitute for a user store, password hashing policy, identity provider, or production secret management.
