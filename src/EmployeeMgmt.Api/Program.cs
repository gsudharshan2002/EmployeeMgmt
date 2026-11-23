using System.Security.Cryptography;
using System.Text;
using EmployeeMgmt.Application.Departments;
using EmployeeMgmt.Application.Employees;
using EmployeeMgmt.Api.Middleware;
using EmployeeMgmt.Api.Security;
using EmployeeMgmt.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration["Authentication:SigningKey"]))
{
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Authentication:SigningKey"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
    });
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a JWT bearer token."
    });
    options.OperationFilter<AuthorizeOperationFilter>();
});
builder.Services.AddOptions<AuthenticationOptions>()
    .BindConfiguration("Authentication")
    .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer)
        && !string.IsNullOrWhiteSpace(options.Audience)
        && options.SigningKey is not null
        && Encoding.UTF8.GetByteCount(options.SigningKey) >= 32,
        "Authentication requires an issuer, audience, and signing key of at least 32 bytes.")
    .ValidateOnStart();
builder.Services.AddOptions<PaginationOptions>()
    .BindConfiguration("Pagination")
    .Validate(options => options.DefaultPageSize > 0 && options.MaxPageSize >= options.DefaultPageSize && options.MaxPageSize <= 500,
        "Pagination limits must be positive, with MaxPageSize at most 500 and not less than DefaultPageSize.")
    .ValidateOnStart();
builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EmployeeDb")));
builder.Services.AddScoped<IEmployeeStore, EfEmployeeStore>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<ITokenIssuer, TokenIssuer>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<AuthenticationOptions>>((options, authenticationOptions) =>
    {
        var authentication = authenticationOptions.Value;
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authentication.Issuer,
            ValidateAudience = true,
            ValidAudience = authentication.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authentication.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
