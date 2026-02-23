namespace Banking.Api.Exceptions;

public abstract class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

// ### 4xx Client Errors

public class BadRequestException(string message) : ApiException(message, 400);
public class UnauthorizedException(string message) : ApiException(message, 401);
public class PaymentRequiredException(string message) : ApiException(message, 402);
public class ForbiddenException(string message) : ApiException(message, 403);
public class NotFoundException(string message) : ApiException(message, 404);
public class MethodNotAllowedException(string message) : ApiException(message, 405);
public class NotAcceptableException(string message) : ApiException(message, 406);
public class ProxyAuthenticationRequiredException(string message) : ApiException(message, 407);
public class RequestTimeoutException(string message) : ApiException(message, 408);
public class ConflictException(string message) : ApiException(message, 409);
public class GoneException(string message) : ApiException(message, 410);
public class LengthRequiredException(string message) : ApiException(message, 411);
public class PreconditionFailedException(string message) : ApiException(message, 412);
public class ContentTooLargeException(string message) : ApiException(message, 413);
public class UriTooLongException(string message) : ApiException(message, 414);
public class UnsupportedMediaTypeException(string message) : ApiException(message, 415);
public class RangeNotSatisfiableException(string message) : ApiException(message, 416);
public class ExpectationFailedException(string message) : ApiException(message, 417);
public class UnprocessableContentException(string message) : ApiException(message, 422);
public class UpgradeRequiredException(string message) : ApiException(message, 426);
public class TooManyRequestsException(string message) : ApiException(message, 429);

// ### 5xx Server Errors

public class InternalServerErrorException(string message) : ApiException(message, 500);
public class NotImplementedException(string message) : ApiException(message, 501);
public class BadGatewayException(string message) : ApiException(message, 502);
public class ServiceUnavailableException(string message) : ApiException(message, 503);
public class GatewayTimeoutException(string message) : ApiException(message, 504);
public class HttpVersionNotSupportedException(string message) : ApiException(message, 505);