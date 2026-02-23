using Banking.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Banking.Api.Exceptions;

/*
 |--------------------------------------------------------------------------------
 | Application Exception Handler
 |--------------------------------------------------------------------------------
 |
 | Handles exceptions thrown from any module and maps them to RFC 9457 compliant
 | problem details responses. All domain and application exception types live in
 | Banking.Shared so every module can throw them without cross-module dependencies.
 |
 */

public class AppExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            // ### Domain exceptions

            AggregateNotFoundException => (404, "Aggregate Not Found"),
            AggregateDeletedException => (410, "Aggregate Deleted"),
            AggregateConflictException => (409, "Aggregate Conflict"),
            InvalidAggregateOperationException => (422, "Invalid Aggregate Operation"),
            DomainInvariantException => (422, "Domain Invariant Violation"),
            DomainValidationException => (400, "Domain Validation Failed"),
            DomainUnauthorizedException => (401, "Unauthorized"),
            DomainForbiddenException => (403, "Forbidden"),

            // ### Api exceptions

            ServiceUnavailableException => (503, "Service Unavailable"),
            ApiException e => (e.StatusCode, e.GetType().Name),

            // ### Default — let ASP.NET Core handle it

            _ => (-1, null)
        };

        if (statusCode == -1)
        {
            return false;
        }

        context.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails =
            {
                Type = GetRfcTypeUrl(statusCode),
                Title = title,
                Status = statusCode,
                Detail = exception.Message
            }
        });
    }

    private static string GetRfcTypeUrl(int statusCode) => statusCode switch
    {
        // ### 4xx Client Errors

        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        402 => "https://tools.ietf.org/html/rfc9110#section-15.5.3",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        405 => "https://tools.ietf.org/html/rfc9110#section-15.5.6",
        406 => "https://tools.ietf.org/html/rfc9110#section-15.5.7",
        407 => "https://tools.ietf.org/html/rfc9110#section-15.5.8",
        408 => "https://tools.ietf.org/html/rfc9110#section-15.5.9",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        410 => "https://tools.ietf.org/html/rfc9110#section-15.5.11",
        411 => "https://tools.ietf.org/html/rfc9110#section-15.5.12",
        412 => "https://tools.ietf.org/html/rfc9110#section-15.5.13",
        413 => "https://tools.ietf.org/html/rfc9110#section-15.5.14",
        414 => "https://tools.ietf.org/html/rfc9110#section-15.5.15",
        415 => "https://tools.ietf.org/html/rfc9110#section-15.5.16",
        416 => "https://tools.ietf.org/html/rfc9110#section-15.5.17",
        417 => "https://tools.ietf.org/html/rfc9110#section-15.5.18",
        422 => "https://tools.ietf.org/html/rfc9110#section-15.5.21",
        426 => "https://tools.ietf.org/html/rfc9110#section-15.5.22",
        429 => "https://tools.ietf.org/html/rfc6585#section-4",

        // ### 5xx Server Errors

        500 => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
        501 => "https://tools.ietf.org/html/rfc9110#section-15.6.2",
        502 => "https://tools.ietf.org/html/rfc9110#section-15.6.3",
        503 => "https://tools.ietf.org/html/rfc9110#section-15.6.4",
        504 => "https://tools.ietf.org/html/rfc9110#section-15.6.5",
        505 => "https://tools.ietf.org/html/rfc9110#section-15.6.6",

        _ => "https://tools.ietf.org/html/rfc9110"
    };
}