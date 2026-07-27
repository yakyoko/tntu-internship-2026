using Microsoft.AspNetCore.Diagnostics;
using Tasks.Api.Exceptions;

namespace Tasks.Api.Infrastructure;

public class ExceptionToProblemDetailsHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ExceptionToProblemDetailsHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        (int statusCode, string title) = exception switch
        {
            ProjectNotFoundException => (StatusCodes.Status404NotFound, "Project not found"),
            TaskNotFoundException => (StatusCodes.Status404NotFound, "Task not found"),
            ProjectArchivedException => (StatusCodes.Status409Conflict, "Project is archived"),
            InvalidTaskStatusTransitionException => (
                StatusCodes.Status409Conflict,
                "Invalid status transition"
            ),
            ProjectApiUnavailableException => (
                StatusCodes.Status502BadGateway,
                "Projects.Api unavailable"
            ),
            _ => (0, string.Empty),
        };

        if (statusCode == 0)
        {
            return false;
        }

        var logLevel =
            statusCode == StatusCodes.Status502BadGateway ? LogLevel.Error : LogLevel.Warning;
        logger.Log(
            logLevel,
            exception,
            "{Title} at {Path}: {Message}",
            title,
            httpContext.Request.Path,
            exception.Message
        );

        httpContext.Response.StatusCode = statusCode;
        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Title = title,
                    Detail = exception.Message,
                    Status = statusCode,
                },
                Exception = exception,
            }
        );
    }
}
