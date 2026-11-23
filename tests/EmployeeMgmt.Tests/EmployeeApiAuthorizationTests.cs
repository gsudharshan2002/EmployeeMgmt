using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmployeeMgmt.Api.Controllers;
using EmployeeMgmt.Application.Employees;
using EmployeeMgmt.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeMgmt.Tests;

public sealed class EmployeeApiAuthorizationTests
{
    [Fact]
    public async Task EmployeeDelete_ReturnsUnauthorizedForbiddenAndNoContentAsAppropriate()
    {
        using var factory = new EmployeeApiFactory();
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<EmployeeDbContext>()
                .Database.MigrateAsync();
        }

        using var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/employees", new EmployeeRequest
        {
            Name = "Alice",
            Salary = 85000m
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var employee = await createResponse.Content.ReadFromJsonAsync<EmployeeDto>();
        Assert.NotNull(employee);

        var unauthenticated = await client.DeleteAsync($"/api/employees/{employee.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticated.StatusCode);

        var employeeToken = await LoginAsync(client, "employee", "employee-password");
        using var forbiddenRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/employees/{employee.Id}");
        forbiddenRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", employeeToken);
        var forbidden = await client.SendAsync(forbiddenRequest);
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);

        var adminToken = await LoginAsync(client, "admin", "admin-password");
        using var authorizedRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/employees/{employee.Id}");
        authorizedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var authorized = await client.SendAsync(authorizedRequest);
        Assert.Equal(HttpStatusCode.NoContent, authorized.StatusCode);
    }

    [Fact]
    public async Task Login_InvalidCredentialsReturnsUnauthorized()
    {
        using var factory = new EmployeeApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/token", new LoginRequest
        {
            Username = "employee",
            Password = "incorrect"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/token", new LoginRequest
        {
            Username = username,
            Password = password
        });
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(token);
        Assert.Equal("Bearer", token.TokenType);
        return token.AccessToken;
    }

    private sealed class EmployeeApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"employeemgmt-{Guid.NewGuid():N}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:EmployeeDb"] = $"Data Source={_databasePath}",
                    ["Authentication:Issuer"] = "EmployeeMgmt.Tests",
                    ["Authentication:Audience"] = "EmployeeMgmt.Tests",
                    ["Authentication:SigningKey"] = "integration-test-key-at-least-32-bytes-long",
                    ["Authentication:AdminUsername"] = "admin",
                    ["Authentication:AdminPassword"] = "admin-password",
                    ["Authentication:UserUsername"] = "employee",
                    ["Authentication:UserPassword"] = "employee-password"
                });
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            foreach (var suffix in new[] { "", "-shm", "-wal" })
            {
                var file = _databasePath + suffix;
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
    }
}
