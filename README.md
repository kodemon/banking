# Banking

## Prerequisites

Ensure the following are installed before proceeding:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js LTS](https://nodejs.org/) (for the Vite + React frontend)
- [Docker](https://docs.docker.com/get-docker/)

---

## Getting Started

### 1. Start Services

Spin up all required services via Docker Compose:

```sh
docker compose up -d
```

### 2. Configure Local HTTPS

Secure authentication requires local HTTPS support. Follow these steps:

**Add a local host entry:**

```sh
sudo nano /etc/hosts
```

Append the following line:

```
127.0.0.1 banking.local
```

**Extract the local CA certificate from the Caddy container:**

```sh
docker compose exec caddy cat /data/caddy/pki/authorities/local/root.crt | tee banking.local.crt > /dev/null
```

**Trust the certificate in Firefox:**

1. Open **Settings → Privacy & Security → View Certificates → Authorities → Import**
2. Select the extracted certificate file: `/path/to/project/banking.local.crt`
3. Check **Trust this CA to identify websites** and confirm

---

## Development

### API

Start the API server:

```sh
dotnet run --project api/Banking.Api
```

### App

From the `app` directory, generate the typed API client:

```sh
npm run build:api
```

This produces `app/src/openapi.gen.ts`, used by [openapi-typescript](https://github.com/openapi-ts/openapi-typescript) and [openapi-fetch](https://github.com/openapi-ts/openapi-typescript/tree/main/packages/openapi-fetch) to provide a type-safe client–server interface.

Then start the development server:

```sh
npm run dev
```

---

## Database Migrations

### Create a Migration

Generate a migration for a specific domain:

```sh
./scripts/migration/create.sh $DOMAIN $MIGRATION_NAME
```

**Example:**

```sh
./scripts/migration/create.sh accounts init
```

### Apply Migrations

With the Docker environment running, apply all pending migrations:

```sh
./scripts/migration/update.sh
```

---

## Utilities

### Format

Run the code formatter from the `/api` directory:

```sh
dotnet csharpier format .
```

### Clean

Remove all generated build artifacts:

```sh
rm -rf **/*/bin/ **/*/obj/ **/*/bin\\Debug/
```