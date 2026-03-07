using Banking.Principals.Events;

namespace Banking.Api.Commands;

/*
 |--------------------------------------------------------------------------------
 | Principal Commands
 |--------------------------------------------------------------------------------
 */

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalCreated"/>.</summary>
public record CreatePrincipalCommand(
    Guid CorrelationId,
    Guid PrincipalId,
    string Provider,
    string ExternalId
);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalDeleted"/>.</summary>
public record DeletePrincipalCommand(Guid CorrelationId, Guid PrincipalId);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalIdentityAdded"/>.</summary>
public record AddPrincipalIdentityCommand(
    Guid CorrelationId,
    Guid PrincipalId,
    string Provider,
    string ExternalId
);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalIdentityRemoved"/>.</summary>
public record RemovePrincipalIdentityCommand(
    Guid CorrelationId,
    Guid PrincipalId,
    string Provider,
    string ExternalId
);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalRoleAdded"/>.</summary>
public record AddPrincipalRoleCommand(Guid CorrelationId, Guid PrincipalId, string Role);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalRoleRemoved"/>.</summary>
public record RemovePrincipalRoleCommand(Guid CorrelationId, Guid PrincipalId, string Role);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalAttributeSet"/>.</summary>
public record SetPrincipalAttributeCommand(
    Guid CorrelationId,
    Guid PrincipalId,
    string Domain,
    string Key,
    string Value
);

/// <summary>Handled by PrincipalCommandHandler — emits <see cref="PrincipalAttributeRemoved"/>.</summary>
public record RemovePrincipalAttributeCommand(
    Guid CorrelationId,
    Guid PrincipalId,
    string Domain,
    string Key
);
