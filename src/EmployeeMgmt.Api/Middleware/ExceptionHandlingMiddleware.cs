using Microsoft.AspNetCore.Mvc;

namespace EmployeeMgmt.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
        }
        catch (ArgumentException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Invalid request", exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, "Request conflict", exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing an API request");
            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "The request could not be completed.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, string title, string detail)
    {
        if (context.Response.HasStarted)
            throw new InvalidOperationException("The response has already started.");

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        }, cancellationToken: context.RequestAborted);
    }
}
