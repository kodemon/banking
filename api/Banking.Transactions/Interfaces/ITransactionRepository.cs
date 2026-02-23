namespace Banking.Transactions;

internal interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);

    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetAllByParticipantAsync(Guid participantId);

    Task SaveChangesAsync();
}