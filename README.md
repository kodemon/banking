# Banking

## Env

```env
SQLITE_DB_CONNECTION=Data Source=data/dev.db
```

## Migrations

### Create Migration

To generate migrations for each domain run:

```sh
./scripts/migration/create.sh $DOMAIN $MIGRATION_NAME
```

Example: `./scripts/migration/create.sh accounts init`

### Apply Migration

Once migration updates has been generated run:

```sh
./scripts/migration/update.sh
```

## Run

To start the API run:

```sh
dotnet run --project api/Banking.Api
```
