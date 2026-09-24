using Sample.Common.DTOs;
using Sample.Common.Exceptions;

namespace Sample.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred");

        var response = new ErrorResponse
        {
            StatusCode = exception switch
            {
                DomainException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                DuplicateKeyException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            },
            Message = exception.Message ?? "An error occurred in the application"
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        await context.Response.WriteAsJsonAsync(response);
    }
}