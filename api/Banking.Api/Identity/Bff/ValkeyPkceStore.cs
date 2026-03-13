using System.Text.Json;
using StackExchange.Redis;

namespace Banking.Api.Identity.Bff;

/*
 |--------------------------------------------------------------------------------
 | Valkey PKCE Store
 |--------------------------------------------------------------------------------
 |
 | ...
 |
 */

internal record PkceEntry(string CodeVerifier, string ReturnPath);

internal interface IPkceStore
{
    Task<PkceEntry?> GetAsync(string stateKey, CancellationToken ct = default);
    Task SetAsync(string stateKey, PkceEntry entry, CancellationToken ct = default);
    Task DeleteAsync(string stateKey, CancellationToken ct = default);
}

internal class ValkeyPkceStore(IConnectionMultiplexer valkey) : IPkceStore
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static string Key(string stateKey) => $"bff:pkce:{stateKey}";

    public async Task<PkceEntry?> GetAsync(string stateKey, CancellationToken ct = default)
    {
        var db = valkey.GetDatabase();
        var raw = await db.StringGetAsync(Key(stateKey));
        if (raw.IsNullOrEmpty)
        {
            return null;
        }
        return JsonSerializer.Deserialize<PkceEntry>(raw.ToString(), JsonOptions);
    }

    public async Task SetAsync(string stateKey, PkceEntry entry, CancellationToken ct = default)
    {
        var db = valkey.GetDatabase();
        var json = JsonSerializer.Serialize(entry, JsonOptions);
        await db.StringSetAsync(Key(stateKey), json, Ttl);
    }

    public async Task DeleteAsync(string stateKey, CancellationToken ct = default)
    {
        var db = valkey.GetDatabase();
        await db.KeyDeleteAsync(Key(stateKey));
    }
}
