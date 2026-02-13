namespace Banking.Domain.AccessControl;

public class SystemRules
{
    private readonly UserAccessControlState _state;

    internal SystemRules(UserAccessControlState state)
    {
        _state = state;
    }

    public bool IsAdministrator()
    {
        return _state.IsSystemAdministrator;
    }
}