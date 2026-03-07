using Banking.Transactions.Events;

namespace Banking.Api.Commands;

/*
 |--------------------------------------------------------------------------------
 | Transaction Commands
 |--------------------------------------------------------------------------------
 */

/// <summary>Handled by TransactionCommandHandler — emits <see cref="DepositCreated"/>.</summary>
public record CreateDepositCommand(
    Guid CorrelationId,
    Guid DestinationParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="WithdrawalCreated"/>.</summary>
public record CreateWithdrawalCommand(
    Guid CorrelationId,
    Guid SourceParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="TransferCreated"/>.</summary>
public record CreateTransferCommand(
    Guid CorrelationId,
    Guid SourceParticipantId,
    Guid DestinationParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="FeeCreated"/>.</summary>
public record CreateFeeCommand(
    Guid CorrelationId,
    Guid ParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="InterestCreated"/>.</summary>
public record CreateInterestCommand(
    Guid CorrelationId,
    Guid ParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="TransactionCompleted"/>.</summary>
public record CompleteTransactionCommand(Guid CorrelationId, Guid TransactionId);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="TransactionFailed"/>.</summary>
public record FailTransactionCommand(Guid CorrelationId, Guid TransactionId, string Reason);

/// <summary>Handled by TransactionCommandHandler — emits <see cref="TransactionReversed"/>.</summary>
public record ReverseTransactionCommand(Guid CorrelationId, Guid TransactionId, string Reason);
