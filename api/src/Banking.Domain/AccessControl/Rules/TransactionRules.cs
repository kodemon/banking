using Banking.Domain.ValueObjects;

namespace Banking.Domain.AccessControl;

public class TransactionRules
{
    private readonly UserAccessControlState _state;

    internal TransactionRules(UserAccessControlState state)
    {
        _state = state;
    }

    public Money? GetApprovalLimit()
    {
        return _state.TransactionApprovalLimit;
    }

    public bool CanApprove(Money amount)
    {
        if (_state.IsSystemAdministrator)
        {
            return true;
        }

        var limit = _state.TransactionApprovalLimit;

        if (limit == null)
        {
            return false;
        }

        if (limit.Currency != amount.Currency)
        {
            return false;
        }

        return amount.Amount <= limit.Amount;
    }
}