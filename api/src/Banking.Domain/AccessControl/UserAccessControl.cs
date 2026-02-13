namespace Banking.Domain.AccessControl;

public class UserAccessControl
{
    public Guid UserId { get; }
    public UserAccessControlState State { get; }

    private SystemRules? _system;
    private TransactionRules? _transaction;
    private AccountRules? _account;

    internal UserAccessControl(Guid userId, UserAccessControlState state)
    {
        UserId = userId;
        State = state;
    }

    public SystemRules System
    {
        get
        {
            _system ??= new SystemRules(State);
            return _system;
        }
    }

    public TransactionRules Transaction
    {
        get
        {
            _transaction ??= new TransactionRules(State);
            return _transaction;
        }
    }

    public AccountRules Account
    {
        get
        {
            _account ??= new AccountRules(State);
            return _account;
        }
    }
}
