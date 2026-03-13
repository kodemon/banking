using System.Text.Json;
using StackExchange.Redis;

namespace Banking.Api.Identity.Bff;

/*
 |--------------------------------------------------------------------------------
 | Valkey Session Store
 |--------------------------------------------------------------------------------
 |
 | ...
 |
 */

internal interface IBffSessionStore
{
    Task<BffSession?> GetAsync(string sessionId, CancellationToken ct = default);
    Task SetAsync(string sessionId, BffSession session, CancellationToken ct = default);
    Task DeleteAsync(string sessionId, CancellationToken ct = default);
}

internal class ValkeySessionStore(IConnectionMultiplexer valkey, IConfiguration configuration)
    : IBffSessionStore
{
    private readonly TimeSpan _ttl = TimeSpan.FromDays(
        configuration.GetValue<int>("Bff:SessionTtlDays", 7)
    );

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static string Key(string sessionId) => $"bff:session:{sessionId}";

    public async Task<BffSession?> GetAsync(string sessionId, CancellationToken ct = default)
    {
        var db = valkey.GetDatabase();
        var raw = await db.StringGetAsync(Key(sessionId));
        if (raw.IsNullOrEmpty)
        {
            return null;
        }
        return JsonSerializer.Deserialize<BffSession>(raw.ToString(), JsonOptions);
    }

    public async Task SetAsync(string sessionId, BffSession session, CancellationToken ct = default)
    {
        var db = valkey.GetDatabase();
        var json = JsonSerializer.Serialize(session, JsonOptions);
        await db.StringSetAsync(Key(sessionId), json, _ttl);
    }

    public async Task DeleteAsync(string sessionId, CancellationToken ct = default)
    {
        var db = valkey.GetDatabase();
        await db.KeyDeleteAsync(Key(sessionId));
    }
}
