import { IndexedDB, type Registrars } from "@valkyr/db";

import { api } from "@/services/api";

import { AccountSchema, accountRegistrar } from "./collections/account";

export const db = new IndexedDB({
  name: "valkyr-db:banking:accounts" as const,
  registrars: [accountRegistrar] satisfies Registrars[],
});

export async function resolveAccounts(): Promise<void> {
  db.collection("accounts").insert(
    await api.accounts.list().then((accounts) => accounts.map((account) => AccountSchema.parse(account))),
  );
}

export async function pushAccounts(accounts: any[]) {
  db.collection("accounts").insert(accounts.map((account) => AccountSchema.parse(account)));
}
