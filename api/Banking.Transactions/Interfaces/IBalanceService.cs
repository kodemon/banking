namespace Banking.Transactions.Interfaces;

public interface IBalanceService
{
    Task<long> GetParticipantIdBalanceAsync(Guid participantId);
}
