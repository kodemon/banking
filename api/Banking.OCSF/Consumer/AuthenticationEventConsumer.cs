using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Banking.OCSF.Database;
using Banking.OCSF.Database.Models;
using Banking.OCSF.Events.Authentication;
using Banking.OCSF.Messaging;
using Banking.OCSF.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Banking.OCSF.Consumer;

/// <summary>
/// Hosted background service that consumes OCSF Authentication events from
/// RabbitMQ and persists them to PostgreSQL with a SHA-256 hash chain.
///
/// Hash chain: Hash(N) = SHA256( Payload(N) + Hash(N-1) )
/// Tampering with any record breaks the hash of every subsequent record.
///
/// Concurrency: prefetchCount=1 ensures strict serial processing, which
/// is required to maintain hash chain integrity. One message at a time,
/// fully acknowledged before the next is delivered.
///
/// Retry: on any processing failure the message is negatively acknowledged
/// and requeued (requeue: true). RabbitMQ will redeliver it — effectively
/// retrying until it succeeds or is manually dead-lettered.
///
/// Idempotency: the unique index on EventId makes redelivery safe.
/// </summary>
internal sealed class AuthenticationEventConsumer(
    RabbitMqConnection rabbitConnection,
    IServiceScopeFactory scopeFactory,
    ILogger<AuthenticationEventConsumer> logger
) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
    };

    // "OCSFAUTH" encoded as a long — used as a PostgreSQL advisory lock key
    // to serialise hash chain writes across all application instances.
    private const long AuditChainLockKey = 0x4F435346_41555448L;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Retry the consumer loop if the connection drops, until shutdown.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Clean shutdown — expected.
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OCSF consumer connection lost. Reconnecting in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(CancellationToken ct)
    {
        var conn = await rabbitConnection.GetConnectionAsync(ct);
        await using var channel = await conn.CreateChannelAsync(cancellationToken: ct);

        await channel.QueueDeclareAsync(
            queue: RabbitMqAuditLogger.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct
        );

        // prefetchCount: 1 — only one unacknowledged message at a time.
        // This is what enforces serial processing for hash chain integrity.
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: ct
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, delivery) =>
        {
            var eventId = "(unknown)";
            try
            {
                var json = Encoding.UTF8.GetString(delivery.Body.Span);
                var @event = JsonSerializer.Deserialize<AuthenticationEvent>(json, JsonOptions);

                if (@event is null)
                {
                    logger.LogWarning("Received null or unparseable OCSF event — discarding");
                    await channel.BasicNackAsync(
                        delivery.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: ct
                    );
                    return;
                }

                eventId = @event.EventId.ToString();

                await PersistAsync(@event, ct);

                await channel.BasicAckAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    cancellationToken: ct
                );

                logger.LogInformation(
                    "Persisted OCSF Authentication event {EventId} (Activity={Activity}, Status={Status})",
                    @event.EventId,
                    @event.Activity,
                    @event.Status
                );
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                // Already persisted — safe to acknowledge and move on.
                logger.LogWarning(
                    "Duplicate OCSF event {EventId} — already persisted, acknowledging",
                    eventId
                );
                await channel.BasicAckAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to persist OCSF event {EventId} — requeueing", eventId);

                // Requeue: true — RabbitMQ will redeliver. Combined with prefetchCount: 1
                // this creates a simple retry loop. For poison messages, configure a
                // dead-letter exchange in RabbitMQ to prevent infinite requeue.
                await channel.BasicNackAsync(
                    delivery.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: ct
                );
            }
        };

        await channel.BasicConsumeAsync(
            queue: RabbitMqAuditLogger.QueueName,
            autoAck: false, // Manual ack — we ack only after successful persistence.
            consumer: consumer,
            cancellationToken: ct
        );

        // Hold the channel open until shutdown or connection loss.
        await Task.Delay(Timeout.Infinite, ct);
    }

    private async Task PersistAsync(AuthenticationEvent @event, CancellationToken ct)
    {
        // Each message gets its own DI scope — OcsfDbContext is scoped.
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<OcsfDbContext>();

        var payload = JsonSerializer.Serialize(@event, JsonOptions);

        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.ReadCommitted,
            ct
        );

        // Advisory lock serialises all writers across instances.
        // Released automatically on transaction commit or rollback.
        await db.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_xact_lock({0})",
            [AuditChainLockKey],
            ct
        );

        var previousHash = await db
            .AuditLog.OrderByDescending(e => e.Id)
            .Select(e => e.Hash)
            .FirstOrDefaultAsync(ct);

        var hash = ComputeHash(payload, previousHash);

        db.AuditLog.Add(
            new AuditLogEntry(
                eventId: @event.EventId,
                classUid: @event.ClassUid,
                categoryUid: @event.CategoryUid,
                occurredAt: @event.OccurredAt,
                payload: payload,
                hash: hash,
                previousHash: previousHash
            )
        );

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    private static string ComputeHash(string payload, string? previousHash)
    {
        var input = payload + (previousHash ?? string.Empty);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException?.Message.Contains("23505") == true;
}
