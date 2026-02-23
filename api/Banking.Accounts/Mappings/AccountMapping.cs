using Banking.Accounts.DTO.Responses;

namespace Banking.Accounts;

internal static class AccountMappings
{
    public static AccountResponse ToResponse(this Account account, long balance) => new()
    {
        Id = account.Id,
        Type = account.Type.ToString(),
        Status = account.Status.ToString(),
        Currency = account.Currency.Code,
        Holders = account.AccountHolders.Select(h => h.ToResponse()).ToList(),
        Balance = balance,
        CreatedAt = account.CreatedAt
    };

    public static AccountHolderResponse ToResponse(this AccountHolder holder) => new()
    {
        Id = holder.Id,
        HolderId = holder.HolderId,
        HolderType = holder.HolderType.ToString(),
        CreatedAt = holder.CreatedAt
    };
}