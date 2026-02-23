using Banking.Application.Exceptions;
using Banking.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Banking.Api.Exceptions;

/*
 |--------------------------------------------------------------------------------
 | Application Exception Handler
 |--------------------------------------------------------------------------------
 |
 | As presented this is a handler for any custom application exceptions thrown
 | or returned from Banking.Application services.
 |
 | It's set up be compliant with the RFC 9457 standard ensuring detailed
 | reporting of exceptions occuring in the request loop.
 |
 */

public class AppExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, title) = exception switch
        {
            // ### Domain exceptions

            AggregateNotFoundException e => (404, "Aggregate Not Found"),
            AggregateDeletedException e => (410, "Aggregate Deleted"),
            AggregateConflictException e => (409, "Aggregate Conflict"),
            InvalidAggregateOperationException e => (422, "Invalid Aggregate Operation"),
            DomainInvariantException e => (422, "Domain Invariant Violation"),
            DomainValidationException e => (400, "Domain Validation Failed"),
            DomainUnauthorizedException e => (401, "Unauthorized"),
            DomainForbiddenException e => (403, "Forbidden"),

            // ### Application exceptions

            ApplicationValidationException e => (400, "Validation Failed"),
            ApplicationConflictException e => (409, "Conflict"),
            RateLimitException e => (429, "Rate Limit Exceeded"),
            ServiceUnavailableException e => (503, "Service Unavailable"),

            // ### Api exceptions

            ApiException e => (e.StatusCode, e.GetType().Name),

            // ### Default

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
