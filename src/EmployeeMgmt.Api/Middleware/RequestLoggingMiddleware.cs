using System.Diagnostics;

namespace EmployeeMgmt.Api.Middleware;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var timer = Stopwatch.StartNew();
        await next(context);
        timer.Stop();
        logger.LogInformation(
            "HTTP {Method} request completed with {StatusCode} in {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Response.StatusCode,
            timer.ElapsedMilliseconds);
    }
}
