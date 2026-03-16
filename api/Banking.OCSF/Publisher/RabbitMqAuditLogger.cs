using System.Text;
using System.Text.Json;
using Banking.OCSF.Events.Authentication;
using Banking.OCSF.Interfaces;
using Banking.OCSF.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Banking.OCSF.Publisher;

/// <summary>
/// Publishes OCSF audit events to RabbitMQ using a durable queue.
///
/// Each publish opens a short-lived channel, publishes with mandatory
/// persistent delivery mode, and disposes the channel. The underlying
/// connection is shared and long-lived via RabbitMqConnection.
///
/// Delivery mode 2 (Persistent) ensures messages survive RabbitMQ restarts.
/// The queue is declared idempotently on each channel open — safe to call
/// repeatedly, no-ops if already exists.
///
/// Exceptions are caught and logged. Audit logging must never fail a
/// business operation — the fire-and-forget contract is intentional.
/// </summary>
internal sealed class RabbitMqAuditLogger(
    RabbitMqConnection connection,
    ILogger<RabbitMqAuditLogger> logger
) : IOcsfAuditLogger
{
    internal const string QueueName = "ocsf.authentication";

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public async Task LogAuthenticationAsync(
        AuthenticationEvent @event,
        CancellationToken ct = default
    )
    {
        try
        {
            var conn = await connection.GetConnectionAsync(ct);
            await using var channel = await conn.CreateChannelAsync(cancellationToken: ct);

            // Declare the queue idempotently — durable survives broker restart,
            // non-exclusive and non-auto-delete for a long-lived audit queue.
            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct
            );

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event, JsonOptions));

            var props = new BasicProperties
            {
                // Persistent delivery mode — survives RabbitMQ restart.
                DeliveryMode = DeliveryModes.Persistent,
                ContentType = "application/json",
                MessageId = @event.EventId.ToString(),
                Timestamp = new AmqpTimestamp(
                    new DateTimeOffset(@event.OccurredAt).ToUnixTimeSeconds()
                ),
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QueueName,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to publish OCSF Authentication event {EventId} to RabbitMQ",
                @event.EventId
            );
        }
    }
}
