using Banking.Shared.ValueObjects;

namespace Banking.Api.Commands;

/*
 |--------------------------------------------------------------------------------
 | Registration Commands
 |--------------------------------------------------------------------------------
 |
 | Prerequisite: the caller must already have an authenticated principal
 | in the system (created automatically on first login by PrincipalProvisioner).
 |
 | RegisterUserCommand orchestrates:
 |   1. CreateUserCommand         → Banking.Users writes user record
 |   2. SetPrincipalAttributeCommand → Banking.Principals links user_id to principal
 |   3. AddPrincipalRoleCommand   → Banking.Principals assigns "user" role
 |
 | No principal is created here — that happens in PrincipalProvisioner during
 | JWT authentication when no principal exists for the Zitadel identity.
 |
 */

public record RegisterUserCommand(
    Guid CorrelationId,
    Guid PrincipalId,
    NameInput Name,
    DateTime DateOfBirth,
    string Email
);
