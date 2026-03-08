using System.Text.Json;
using Banking.Atomic.Interfaces;

namespace Banking.Atomic.Libraries;

public static class AtomicTask
{
    public static AtomicTask<TResult, TRollback> Create<TResult, TRollback>(
        string name,
        Func<Task<TResult>> commit,
        Func<TResult, TRollback> rollback
    ) => new(name, commit, rollback);
}

public class AtomicTask<TResult, TRollback> : IAtomicTask
{
    private readonly Func<Task<TResult>> _commit;
    private readonly Func<TResult, TRollback> _rollback;

    private TResult? _result;
    private TRollback? _rollbackData;
    private bool _hasResult = false;

    public string Name { get; }

    internal TResult Result
    {
        get
        {
            if (_hasResult == false)
            {
                throw new InvalidOperationException($"Task '{Name}' has not committed yet.");
            }
            return _result!;
        }
    }

    public AtomicTask(string name, Func<Task<TResult>> commit, Func<TResult, TRollback> rollback)
    {
        Name = name;
        _commit = commit ?? throw new ArgumentNullException(nameof(commit));
        _rollback = rollback ?? throw new ArgumentNullException(nameof(rollback));
    }

    public async Task ExecuteCommitAsync()
    {
        _result = await _commit();
        _rollbackData = _rollback(_result);
        _hasResult = true;
    }

    public string GetRollbackJson()
    {
        if (_hasResult == false)
        {
            throw new InvalidOperationException($"Task '{Name}' has not committed yet.");
        }
        return JsonSerializer.Serialize(_rollbackData);
    }

    public string GetRollbackType() => typeof(TRollback).AssemblyQualifiedName!;
}
