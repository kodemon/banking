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
        PersonalHolders = account.PersonalHolders.Select(h => h.ToResponse()).ToList(),
        BusinessHolders = account.BusinessHolders.Select(h => h.ToResponse()).ToList(),
        Balance = balance,
        CreatedAt = account.CreatedAt
    };

    public static PersonalHolderResponse ToResponse(this PersonalAccountHolder holder) => new()
    {
        Id = holder.Id,
        UserId = holder.UserId,
        HolderType = holder.HolderType.ToString(),
        CreatedAt = holder.CreatedAt
    };

    public static BusinessHolderResponse ToResponse(this BusinessAccountHolder holder) => new()
    {
        Id = holder.Id,
        BusinessId = holder.BusinessId,
        HolderType = holder.HolderType.ToString(),
        CreatedAt = holder.CreatedAt
    };
}