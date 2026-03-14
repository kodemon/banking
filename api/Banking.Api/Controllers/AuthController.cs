using System.Buffers.Text;
using System.Text.Json;
using Banking.Api.Identity;
using Banking.Principals.Commands;
using Banking.Principals.Queries;
using Fido2NetLib;
using Fido2NetLib.Objects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

/*
 |--------------------------------------------------------------------------------
 | Auth Controller
 |--------------------------------------------------------------------------------
 |
 | Passkey authentication using Fido2NetLib — no external identity provider.
 | Credentials and sessions are stored in the principals database.
 |
 | Registration flow:
 |   POST /api/auth/passkey/registration/begin   — generate creation options
 |   POST /api/auth/passkey/registration/verify  — verify + store credential, set session
 |
 | Login flow:
 |   POST /api/auth/passkey/login/begin          — generate assertion options
 |   POST /api/auth/passkey/login/verify         — verify assertion, set session
 |
 | Session:
 |   GET  /api/auth/session                      — validate current session
 |   POST /api/auth/logout                       — delete session + clear cookie
 |
 */

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
public class AuthController(
    IFido2 fido2,
    IMemoryCache cache,
    IMediator mediator,
    ILogger<AuthController> logger
) : ControllerBase
{
    private static readonly TimeSpan ChallengeTtl = TimeSpan.FromMinutes(5);

    // -------------------------------------------------------------------------
    // POST /api/auth/passkey/registration/begin
    // -------------------------------------------------------------------------

    [HttpPost("passkey/registration/begin")]
    [ProducesResponseType<PasskeyRegistrationBeginResponse>(200)]
    [ProducesResponseType(400)]
    public IActionResult PasskeyRegistrationBegin([FromBody] PasskeyRegistrationBeginRequest body)
    {
        var userId = Guid.NewGuid();
        var email = body.Email;
        var displayName = "Banking";

        var user = new Fido2User
        {
            Id = userId.ToByteArray(),
            Name = email,
            DisplayName = displayName,
        };

        var options = fido2.RequestNewCredential(
            new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = [],
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    ResidentKey = ResidentKeyRequirement.Required,
                    UserVerification = UserVerificationRequirement.Required,
                },
                AttestationPreference = AttestationConveyancePreference.None,
            }
        );

        cache.Set($"reg:{userId}", options, ChallengeTtl);

        return Ok(new PasskeyRegistrationBeginResponse(userId.ToString(), options));
    }

    // -------------------------------------------------------------------------
    // POST /api/auth/passkey/registration/verify
    // -------------------------------------------------------------------------

    [HttpPost("passkey/registration/verify")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PasskeyRegistrationVerify(
        [FromBody] PasskeyRegistrationVerifyRequest body,
        CancellationToken ct
    )
    {
        if (
            !cache.TryGetValue($"reg:{body.UserId}", out CredentialCreateOptions? options)
            || options is null
        )
        {
            return BadRequest("Registration session expired. Please try again.");
        }

        cache.Remove($"reg:{body.UserId}");

        AuthenticatorAttestationRawResponse attestation;
        try
        {
            attestation =
                JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(
                    body.AttestationResponse
                ) ?? throw new Exception("Invalid attestation response.");
        }
        catch
        {
            return BadRequest("Invalid attestation response.");
        }

        RegisteredPublicKeyCredential credential;
        try
        {
            credential = await fido2.MakeNewCredentialAsync(
                new MakeNewCredentialParams
                {
                    AttestationResponse = attestation,
                    OriginalOptions = options,
                    IsCredentialIdUniqueToUserCallback = async (args, _) =>
                    {
                        var existing = await mediator.Send(
                            new GetPasskeyCredentialByCredentialIdQuery(
                                Base64Url.EncodeToString(args.CredentialId)
                            ),
                            ct
                        );
                        return existing is null;
                    },
                },
                ct
            );
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                "[Auth:PasskeyRegistrationVerify] Fido2 verification failed: {Message}",
                ex.Message
            );
            return BadRequest("Passkey verification failed. Please try again.");
        }

        var credentialIdBase64 = Base64Url.EncodeToString(credential.Id);

        var principal = await mediator.Send(
            new CreatePrincipalCommand("passkey", credentialIdBase64),
            ct
        );

        await mediator.Send(
            new RegisterPasskeyCredentialCommand(
                principal.Id,
                credentialIdBase64,
                credential.PublicKey,
                credential.SignCount,
                body.CredentialName,
                credential.AaGuid
            ),
            ct
        );

        var session = await mediator.Send(new CreateSessionCommand(principal.Id), ct);

        AuthMiddleware.SetSessionCookie(Response, session.Id);

        return Ok();
    }

    // -------------------------------------------------------------------------
    // POST /api/auth/passkey/login/begin
    // -------------------------------------------------------------------------

    [HttpPost("passkey/login/begin")]
    [ProducesResponseType<PasskeyLoginBeginResponse>(200)]
    public IActionResult PasskeyLoginBegin([FromBody] PasskeyLoginBeginRequest body)
    {
        var options = fido2.GetAssertionOptions(
            new GetAssertionOptionsParams
            {
                AllowedCredentials = [],
                UserVerification = UserVerificationRequirement.Required,
            }
        );

        cache.Set($"login:{body.SessionKey}", options, ChallengeTtl);

        return Ok(new PasskeyLoginBeginResponse(body.SessionKey, options));
    }

    // -------------------------------------------------------------------------
    // POST /api/auth/passkey/login/verify
    // -------------------------------------------------------------------------

    [HttpPost("passkey/login/verify")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> PasskeyLoginVerify(
        [FromBody] PasskeyLoginVerifyRequest body,
        CancellationToken ct
    )
    {
        if (
            !cache.TryGetValue($"login:{body.SessionKey}", out AssertionOptions? options)
            || options is null
        )
        {
            return Unauthorized("Login session expired. Please try again.");
        }

        cache.Remove($"login:{body.SessionKey}");

        AuthenticatorAssertionRawResponse assertion;
        try
        {
            assertion =
                JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(
                    body.AssertionResponse
                ) ?? throw new Exception("Invalid assertion response.");
        }
        catch
        {
            return Unauthorized("Invalid assertion response.");
        }

        var credentialIdBase64 = assertion.Id;
        var storedCredential = await mediator.Send(
            new GetPasskeyCredentialByCredentialIdQuery(credentialIdBase64),
            ct
        );

        if (storedCredential is null)
        {
            return Unauthorized("Passkey not recognised.");
        }

        VerifyAssertionResult result;
        try
        {
            result = await fido2.MakeAssertionAsync(
                new MakeAssertionParams
                {
                    AssertionResponse = assertion,
                    OriginalOptions = options,
                    StoredPublicKey = storedCredential.PublicKey,
                    StoredSignatureCounter = storedCredential.SignCount,
                    IsUserHandleOwnerOfCredentialIdCallback = (args, _) =>
                        Task.FromResult(
                            Base64Url.EncodeToString(args.CredentialId)
                                == storedCredential.CredentialId
                        ),
                },
                ct
            );
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                "[Auth:PasskeyLoginVerify] Fido2 assertion failed: {Message}",
                ex.Message
            );
            return Unauthorized("Passkey verification failed.");
        }

        await mediator.Send(
            new UpdatePasskeySignCountCommand(credentialIdBase64, result.SignCount),
            ct
        );

        var session = await mediator.Send(
            new CreateSessionCommand(storedCredential.PrincipalId),
            ct
        );

        AuthMiddleware.SetSessionCookie(Response, session.Id);

        return Ok();
    }

    // -------------------------------------------------------------------------
    // POST /api/auth/logout
    // -------------------------------------------------------------------------

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var sessionId = AuthMiddleware.GetSessionId(Request);
        if (sessionId.HasValue)
        {
            await mediator.Send(new DeleteSessionCommand(sessionId.Value), ct);
        }

        AuthMiddleware.ClearSessionCookie(HttpContext);
        return Ok();
    }

    // -------------------------------------------------------------------------
    // GET /api/auth/session
    // -------------------------------------------------------------------------

    [HttpGet("session")]
    [ProducesResponseType<SessionResponse>(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Session(CancellationToken ct)
    {
        var sessionId = AuthMiddleware.GetSessionId(Request);
        if (!sessionId.HasValue)
        {
            return Unauthorized();
        }

        var session = await mediator.Send(new GetSessionByIdQuery(sessionId.Value), ct);
        if (session is null || session.IsExpired)
        {
            return Unauthorized();
        }

        return Ok(new SessionResponse(session.PrincipalId));
    }
}

// ---------------------------------------------------------------------------
// Request / response shapes
// ---------------------------------------------------------------------------

public record PasskeyRegistrationBeginRequest(string Email);

public record PasskeyRegistrationBeginResponse(string UserId, CredentialCreateOptions Options);

public record PasskeyRegistrationVerifyRequest(
    string UserId,
    string CredentialName,
    JsonElement AttestationResponse
);

public record PasskeyLoginBeginRequest(string SessionKey);

public record PasskeyLoginBeginResponse(string SessionKey, AssertionOptions Options);

public record PasskeyLoginVerifyRequest(string SessionKey, JsonElement AssertionResponse);

public record SessionResponse(Guid PrincipalId);
