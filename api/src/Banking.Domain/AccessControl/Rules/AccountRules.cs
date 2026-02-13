using Banking.Domain.Entities;

namespace Banking.Domain.AccessControl;

public class AccountRules
{
    private readonly UserAccessControlState _state;

    internal AccountRules(UserAccessControlState state)
    {
        _state = state;
    }

    public bool CanAccess(Account account)
    {
        if (_state.IsSystemAdministrator)
        {
            return true;
        }
        if (_state.AccessibleAccountIds.Contains(account.Id))
        {
            return true;
        }
        return false;
    }

    public bool CanAccess(Guid accountId)
    {
        if (_state.IsSystemAdministrator)
        {
            return true;
        }
        return _state.AccessibleAccountIds.Contains(accountId);
    }

    public bool Owns(AccountHolder holder)
    {
        if (_state.IsSystemAdministrator)
        {
            return true;
        }
        return _state.AccessibleAccountHolderIds.Contains(holder.Id);
    }

    public bool Owns(Guid accountHolderId)
    {
        if (_state.IsSystemAdministrator)
        {
            return true;
        }
        return _state.AccessibleAccountHolderIds.Contains(accountHolderId);
    }
}
