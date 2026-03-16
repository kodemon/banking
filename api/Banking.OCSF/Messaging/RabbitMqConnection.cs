using RabbitMQ.Client;

namespace Banking.OCSF.Messaging;

/// <summary>
/// Holds a single long-lived RabbitMQ connection for the lifetime of the application.
///
/// RabbitMQ.Client v7 connections are thread-safe. Channels (IChannel) are not —
/// each publisher or consumer operation creates and disposes its own channel.
///
/// The connection is established lazily on first use and re-established automatically
/// by the publisher if it detects the connection has been closed.
/// </summary>
internal sealed class RabbitMqConnection(ConnectionFactory factory) : IAsyncDisposable
{
    private IConnection? _connection;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<IConnection> GetConnectionAsync(CancellationToken ct)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _lock.WaitAsync(ct);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            _connection?.Dispose();
            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
