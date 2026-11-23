using EmployeeMgmt.Application.Departments;
using EmployeeMgmt.Application.Employees;
using EmployeeMgmt.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<PaginationOptions>()
    .BindConfiguration("Pagination")
    .Validate(options => options.DefaultPageSize > 0 && options.MaxPageSize >= options.DefaultPageSize && options.MaxPageSize <= 500,
        "Pagination limits must be positive, with MaxPageSize at most 500 and not less than DefaultPageSize.")
    .ValidateOnStart();
builder.Services.AddSingleton<IEmployeeStore, InMemoryEmployeeStore>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program;
