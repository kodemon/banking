#!/bin/bash
set -e

declare -A PROJECTS=(
    ["accounts"]="api/Banking.Accounts"
    ["events"]="api/Banking.Events"
    ["principals"]="api/Banking.Principals"
    ["transactions"]="api/Banking.Transactions"
    ["users"]="api/Banking.Users"
)

DOMAIN=$1
MIGRATION_NAME=$2

if [ -z "$DOMAIN" ] || [ -z "$MIGRATION_NAME" ]; then
    echo "Usage: ./scripts/migration/create.sh <domain> <migration-name>"
    echo "Domains: ${!PROJECTS[@]}"
    exit 1
fi

PROJECT=${PROJECTS[$DOMAIN]}

if [ -z "$PROJECT" ]; then
    echo "Unknown domain '$DOMAIN'. Available domains: ${!PROJECTS[@]}"
    exit 1
fi

echo "[$PROJECT] Creating migration '$MIGRATION_NAME'..."

dotnet ef migrations add "${DOMAIN^}-$MIGRATION_NAME" \
    --project "$PROJECT" \
    --output-dir Persistence/Migrations

echo "[$PROJECT] Migration '$MIGRATION_NAME' created."