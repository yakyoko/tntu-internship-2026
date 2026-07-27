using Microsoft.AspNetCore.Diagnostics;
using Projects.Api.Exceptions;

namespace Projects.Api.Infrastructure;

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
            ProjectArchivedException => (StatusCodes.Status409Conflict, "Project is archived"),
            _ => (0, string.Empty),
        };

        if (statusCode == 0)
        {
            return false;
        }

        logger.Log(
            LogLevel.Warning,
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
