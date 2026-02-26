namespace Banking.Shared.Exceptions;

public class AttributeValidationException(string message) : Exception(message);