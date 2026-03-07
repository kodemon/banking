using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Zitadel.Credentials;

namespace Banking.Principals.AccessControl;

internal class ZitadelMetadataService(
    HttpClient http,
    ServiceAccount serviceAccount,
    string authority
)
{
    public async Task SetPrincipalIdAsync(string zitadelUserId, Guid principalId)
    {
        var token = await serviceAccount.AuthenticateAsync(
            audience: authority,
            authOptions: new ServiceAccount.AuthOptions { ApiAccess = true }
        );

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var value = Convert.ToBase64String(Encoding.UTF8.GetBytes(principalId.ToString()));
        var content = new StringContent(
            JsonSerializer.Serialize(new { value }),
            Encoding.UTF8,
            "application/json"
        );
        var response = await http.PostAsync(
            $"/management/v1/users/{zitadelUserId}/metadata/principalId",
            content
        );

        response.EnsureSuccessStatusCode();
    }
}
