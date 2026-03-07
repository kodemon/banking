#!/bin/bash

init_db_files() {
    local db_path=$(grep SQLITE_DB_PATH .env | cut -d'=' -f2 | tr -d '"' | tr -d "'")
    echo "Initializing database files at $db_path..."
    touch "$db_path/accounts.db"
    touch "$db_path/events.db"
    touch "$db_path/principals.db"
    touch "$db_path/transactions.db"
    touch "$db_path/users.db"
}

run_migration() {
    local project=$1
    local tmpfile=$(mktemp)

    echo "[$project] Starting migration..."

    dotnet ef database update --project "$project" > "$tmpfile" 2>&1
    local exit_code=$?

    local output=$(cat "$tmpfile")
    rm "$tmpfile"

    if [ $exit_code -ne 0 ]; then
        echo "[$project] Migration failed:"
        echo "$output"
        exit $exit_code
    fi

    if echo "$output" | grep -q "No migrations were applied. The database is already up to date."; then
        echo "[$project] Already up to date, skipping."
    else
        echo "$output"
        echo "[$project] Migration complete."
    fi
}

run_migration api/Banking.Accounts
run_migration api/Banking.Events
run_migration api/Banking.Principals
run_migration api/Banking.Transactions
run_migration api/Banking.Users

echo "All migrations completed."