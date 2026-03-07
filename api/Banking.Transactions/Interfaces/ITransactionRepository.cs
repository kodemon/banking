using Banking.Transactions.Repositories.Resources;

namespace Banking.Transactions.Interfaces;

internal interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);

    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetAllByParticipantAsync(Guid participantId);

    Task SaveChangesAsync();
}
