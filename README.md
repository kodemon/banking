# Banking

## Format

From the `/api` folder run:

```sh
dotnet csharpier format .
```

## Clean

Clean out all the generated .NET folders

```sh
rm -rf  **/*/bin/ **/*/obj/ **/*/bin\\Debug/
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
