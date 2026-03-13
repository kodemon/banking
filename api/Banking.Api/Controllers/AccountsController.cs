using Banking.Accounts.Commands;
using Banking.Accounts.Enums;
using Banking.Accounts.Queries;
using Banking.Accounts.Repositories.Resources;
using Banking.Api.Identity;
using Banking.Shared.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/accounts")]
[Authorize]
[Tags("Account")]
internal class AccountsController(IAuth auth, IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<IEnumerable<Account>>(200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(409)]
    public async Task<ActionResult> Create(CreateAccountPayload payload)
    {
        var allowed = await auth.IsAllowed("account", "*", "create");
        if (allowed == false)
        {
            return Forbid();
        }

        var account = await mediator.Send(
            new CreateAccountCommand(
                payload.AccountName,
                payload.AccountType,
                Currency.FromCode(payload.Currency),
                auth.Principal.Id,
                payload.HolderType
            )
        );

        return Ok(
            new AccountResponse(
                account.Id,
                account.Number.ToString(),
                account.Name,
                account.Type,
                account.Status,
                account.Currency.Code,
                account.AccountHolders.Select(
                    (holder) =>
                        new AccountHolderResponse(
                            holder.Id,
                            holder.AccountId,
                            holder.HolderId,
                            holder.HolderType,
                            holder.CreatedAt
                        )
                ),
                account.CreatedAt
            )
        );
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<AccountResponse>>(200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(409)]
    public async Task<ActionResult> Accounts()
    {
        var accounts = await mediator.Send(new GetAccountsByOwnerIdQuery(auth.Principal.Id));

        // TODO: Check that principal has access to all the retrieved accounts
        // var allowed = await auth.IsAllowed("account", "*", "view");
        // if (allowed == false)
        // {
        //     return Forbid();
        // }

        return Ok(
            accounts.Select(
                (account) =>
                    new AccountResponse(
                        account.Id,
                        account.Number.ToString(),
                        account.Name,
                        account.Type,
                        account.Status,
                        account.Currency.Code,
                        account.AccountHolders.Select(
                            (holder) =>
                                new AccountHolderResponse(
                                    holder.Id,
                                    holder.AccountId,
                                    holder.HolderId,
                                    holder.HolderType,
                                    holder.CreatedAt
                                )
                        ),
                        account.CreatedAt
                    )
            )
        );
    }
}

internal record CreateAccountPayload
{
    public required string AccountName { get; init; }
    public required AccountType AccountType { get; init; }
    public required string Currency { get; init; }
    public required AccountHolderType HolderType { get; init; }
}

internal record AccountResponse(
    Guid Id,
    string Number,
    string Name,
    AccountType Type,
    AccountStatus Status,
    string Currency,
    IEnumerable<AccountHolderResponse> Holders,
    DateTime CreatedAt
);

internal record AccountHolderResponse(
    Guid Id,
    Guid AccountId,
    Guid HolderId,
    AccountHolderType Type,
    DateTime CreatedAt
);
