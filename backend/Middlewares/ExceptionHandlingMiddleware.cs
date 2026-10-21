using System.Net;
using SalesCubeApi.Models;

namespace SalesCubeApi.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Request cancelled: {Path}", context.Request.Path);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid analytics request: {Path}", context.Request.Path);
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, "Invalid request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "SSAS configuration or execution failure: {Path}", context.Request.Path);
            await WriteErrorAsync(context, HttpStatusCode.ServiceUnavailable, "Data source unavailable", "Unable to access the configured SSAS data source. Check the backend logs.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error on {Path}", context.Request.Path);
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "Server error", "The request could not be completed. Check the backend logs.");
        }
    }

    private static Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string error, string message)
    {
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(new ApiErrorDto(
            StatusCode: (int)statusCode,
            Error: error,
            Message: message,
            TimestampUtc: DateTime.UtcNow
        ));
    }
}
