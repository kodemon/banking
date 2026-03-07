using Banking.Accounts.Events;

namespace Banking.Api.Commands;

/*
 |--------------------------------------------------------------------------------
 | Account Commands
 |--------------------------------------------------------------------------------
 */

/// <summary>Handled by AccountCommandHandler — emits <see cref="AccountCreated"/>.</summary>
public record CreateAccountCommand(
    Guid CorrelationId,
    string AccountType,
    string CurrencyCode,
    Guid PrimaryHolderId,
    string PrimaryHolderType
);

/// <summary>Handled by AccountCommandHandler — emits <see cref="AccountHolderAdded"/>.</summary>
public record AddAccountHolderCommand(
    Guid CorrelationId,
    Guid AccountId,
    Guid HolderId,
    string HolderType
);

/// <summary>Handled by AccountCommandHandler — emits <see cref="AccountHolderRemoved"/>.</summary>
public record RemoveAccountHolderCommand(Guid CorrelationId, Guid AccountId, Guid HolderId);

/// <summary>Handled by AccountCommandHandler — emits <see cref="AccountFrozen"/>.</summary>
public record FreezeAccountCommand(Guid CorrelationId, Guid AccountId);

/// <summary>Handled by AccountCommandHandler — emits <see cref="AccountUnfrozen"/>.</summary>
public record UnfreezeAccountCommand(Guid CorrelationId, Guid AccountId);

/// <summary>Handled by AccountCommandHandler — emits <see cref="AccountClosed"/>.</summary>
public record CloseAccountCommand(Guid CorrelationId, Guid AccountId);
