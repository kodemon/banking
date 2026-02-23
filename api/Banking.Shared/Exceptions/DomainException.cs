namespace Banking.Shared.Exceptions;

public abstract class DomainException(string message) : Exception(message);

// ### Aggregate & Entity

public class AggregateNotFoundException(string message) : DomainException(message);
public class AggregateDeletedException(string message) : DomainException(message);
public class AggregateConflictException(string message) : DomainException(message);
public class InvalidAggregateOperationException(string message) : DomainException(message);

// ### Invariants & Validation

public class DomainInvariantException(string message) : DomainException(message);
public class DomainValidationException(string message) : DomainException(message);

// ### Access

public class DomainUnauthorizedException(string message) : DomainException(message);
public class DomainForbiddenException(string message) : DomainException(message);