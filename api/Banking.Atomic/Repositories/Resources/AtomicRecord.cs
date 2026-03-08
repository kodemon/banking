namespace Banking.Atomic.Repositories.Resources;

public class AtomicRecord
{
    public Guid Id { get; init; }
    public Guid FlowId { get; init; }

    public string TaskName { get; init; } = string.Empty;
    public string RollbackJson { get; init; } = string.Empty;
    public string RollbackType { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
