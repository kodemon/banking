# Banking

## Linux Commands

```sh
find . -name "*.cs" -type f -exec echo "=== {} ===" \; -exec cat {} \; -exec echo "" \;
```

# Banking.Api

## Run

To start the API run:

```sh
dotnet run --project api/src/Banking.Api
```

# Banking.Infrastructure

## Migrations

### Create Migration

From the /api folder in your terminal run:

```sh
$ dotnet ef migrations add Initial \
  --project src/Banking.Infrastructure \
  --startup-project src/Banking.Api \
  --output-dir Persistence/Migrations
```

### Apply Migration

Once migration updates has been generated run:

```sh
$ dotnet ef database update \
  --project src/Banking.Infrastructure \
  --startup-project src/Banking.Api
```
