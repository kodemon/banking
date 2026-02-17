namespace Banking.Application.Exceptions;

public abstract class ApplicationException(string message) : Exception(message);

public class ApplicationValidationException(string message) : ApplicationException(message);
public class ApplicationConflictException(string message) : ApplicationException(message);
public class RateLimitException(string message) : ApplicationException(message);
public class ServiceUnavailableException(string message) : ApplicationException(message);
