# Banking

# Migrations

### Create Migration

To generate migrations for each domain run:

```sh
dotnet ef migrations add InitialAccounts --project api/Banking.Accounts --output-dir Persistence/Migrations
dotnet ef migrations add InitialTransactions --project api/Banking.Transactions --output-dir Persistence/Migrations
dotnet ef migrations add InitialUsers --project api/Banking.Users --output-dir Persistence/Migrations
```

### Apply Migration

Once migration updates has been generated run:

```sh
dotnet ef database update --project api/Banking.Accounts
dotnet ef database update --project api/Banking.Transactions
dotnet ef database update --project api/Banking.Users
```

# Run

To start the API run:

```sh
dotnet run --project api/Banking.Api
```
