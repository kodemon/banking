## Zitadel Setup

The API uses Zitadel's [Session v2 API](https://zitadel.com/docs/apis/resources/session_service_v2) as the sole source of truth for session state. There is no local session store — every request is validated directly against Zitadel. The following setup is required before the API will start correctly.

Setup is split across two organisational contexts. The **root instance organisation** is the top-level org Zitadel provisions automatically — it owns instance-wide configuration and the `login-client` service user. Your **application organisation** is the org where your project and its applications live. These are separate concerns and the steps below make clear which context each action belongs to.

---

### Root Instance Organisation

All steps in this section are performed in the root instance organisation.

#### 1. Locate the Login Client Service User

Zitadel automatically provisions a service user called `login-client` with the `IAM_LOGIN_CLIENT` role on new installations. This is the intended account for connecting a custom login UI to the Session v2 API — use it directly rather than creating a separate service user.

Navigate to the root instance organisation and go to **Users → Service Users**. You should see `login-client` already listed there.

> If `login-client` is missing (e.g. on an older installation that predates automatic provisioning), create one manually: go to **Users → Service Users → New Service User**, set the name to `login-client`, set **Access Token Type** to **Bearer**, then follow step 2 below to assign it the `IAM_LOGIN_CLIENT` role at instance level.

#### 2. Verify IAM Permissions

The `login-client` user should already have the `IAM_LOGIN_CLIENT` role assigned at instance level. Confirm this before generating a token.

1. Navigate to **Instance → Instance Members**.
2. Find `login-client` in the member list and confirm its role is **`IAM_LOGIN_CLIENT`**.

If it is missing, click **Add Member**, search for `login-client`, assign the role **`IAM_LOGIN_CLIENT`**, and save.

> **Why instance-level?** The Session v2 API validates passkey credentials against the instance's WebAuthn configuration. Organisation-scoped roles are not sufficient for session write operations.

#### 3. Enable Feature Flags

The Session v2 API and the User v2 API are gated behind feature flags at the instance level. Both must be enabled before the passkey flow or any v2 API call will work.

Navigate to **Instance → Settings → Features**, or go directly to:

```
https://<your-zitadel-domain>/ui/console/settings
```

Enable both of the following flags:

| Flag | Why it's needed |
|------|-----------------|
| **Use V2 API in Management Console for User creation** | Enables the User v2 API used to look up users by login name during the passkey begin step. Without this, user lookup falls back to the deprecated v1 API. |
| **Login V2** | Unlocks the Session v2 API endpoints (`POST /v2/sessions`, `PATCH /v2/sessions/:id`, `GET /v2/sessions/:id`) that the entire passkey authentication flow depends on. |

Each flag can be set to `Enabled`, `Disabled`, or `Inherit`. Set both to **Enabled**.

> **Note:** These flags apply to all organisations within the instance. Take care on shared or production instances — the Login V2 flag affects all authentication flows.

#### 4. Generate a Personal Access Token (PAT)

1. Open the `login-client` service user.
2. Go to the **Personal Access Tokens** tab and click **New Access Token**.
3. Set an expiry appropriate for your environment (or leave blank for no expiry in development).
4. Copy the token immediately — Zitadel will not show it again.

With the token still in your clipboard, store it as a .NET user secret now:

```sh
dotnet user-secrets set "Zitadel:ServiceAccountToken" "<paste-token-here>" \
  --project api/Banking.Api
```

---

### Application Organisation

All steps in this section are performed in your application organisation, inside your project.

#### 5. Register a Passkey Application

1. Navigate to your **Project** and click **New Application**.
2. Choose **User Agent** (a browser-based SPA — no client secret).
3. Set **Authentication Method** to **None**. User Agent apps are public clients — they run in the browser and have no safe place to store a secret, so `Basic`, `Post`, and `Private Key JWT` are not applicable.
4. In the **OIDC Configuration**, ensure **PKCE** is enabled. This is separate from the Authentication Method — PKCE is the code challenge mechanism that secures the authorisation code exchange in place of a client secret.
5. Under **Redirect URIs**, leave this empty — the passkey flow does not use OIDC redirects.
6. Note the **Client ID** on the application overview page — this maps to `Zitadel:Audience` in `appsettings.json`.

#### 6. Configure the WebAuthn Domain

Passkeys are bound to the domain they are registered on. Zitadel needs to know your application's domain to generate valid WebAuthn challenges.

1. Navigate to **Instance → Verified Domains**.
2. Add your application domain (e.g. `localhost` for development, `app.example.com` for production) as a trusted domain.

The `Zitadel:Domain` value in `appsettings.json` must match exactly what the browser sees in the address bar when the passkey ceremony runs.

> **Note:** Passkey credentials are cryptographically bound to the domain they were registered on. If you change or add a domain later, existing passkeys registered under the old domain will stop working for users authenticating from the new one.

---

### Local Development

#### 7. Managing the .NET User Secret

The PAT is stored outside the repository using the .NET secrets manager (see step 4). The project's `UserSecretsId` is `0fa84f53-71e3-4954-8c5a-c5052d6d372f`.

To verify the secret was stored:

```sh
dotnet user-secrets list --project api/Banking.Api
```

To rotate it (e.g. after generating a new PAT):

```sh
dotnet user-secrets set "Zitadel:ServiceAccountToken" "<new-token>" \
  --project api/Banking.Api
```

To remove it:

```sh
dotnet user-secrets remove "Zitadel:ServiceAccountToken" \
  --project api/Banking.Api
```

Secrets are stored on disk outside the repository at:

- **macOS/Linux:** `~/.microsoft/usersecrets/0fa84f53-71e3-4954-8c5a-c5052d6d372f/secrets.json`
- **Windows:** `%APPDATA%\Microsoft\UserSecrets\0fa84f53-71e3-4954-8c5a-c5052d6d372f\secrets.json`

---

### Production

#### 8. Production Configuration

Set the token via an environment variable or your secret management platform — do not use `appsettings.json` or user secrets in deployed environments.

**Environment variable:**

```sh
Zitadel__ServiceAccountToken=<your-pat-here>
```

Note the double underscore (`__`) — this is how .NET maps environment variables to nested configuration keys.

**Full configuration reference:**

| Key | Secret | Description | Example |
|-----|--------|-------------|---------|
| `Zitadel:Authority` | No | Base URL of your Zitadel instance | `https://iam.example.com` |
| `Zitadel:Audience` | No | Client ID of the User Agent application — public identifier, not sensitive | `363944439091101698` |
| `Zitadel:Domain` | No | WebAuthn RP domain — must match the browser's address bar | `localhost` |
| `Zitadel:ServiceAccountToken` | **Yes** | PAT for the `login-client` service user — **never commit this** | _(set via secrets)_ |