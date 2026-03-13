import { Controller } from "@/libraries/controller";
import { api } from "@/services/api";
import { session } from "@/services/session";

import { db } from "../database/dbcontext";
import { AccountType } from "../enums/account-type";

export class AccountListController extends Controller<{
  grouped: Grouped;
}> {
  #subscription?: any;

  async onInit() {
    this.#subscribe();
    return {
      grouped: [],
    };
  }

  async onDestroy(): Promise<void> {
    this.#subscription?.unsubscribe();
  }

  async #subscribe() {
    this.#subscription = db
      .collection("accounts")
      .subscribe({ "holders.holderId": await session.getPrincipalId() }, {}, (accounts) => {
        this.setState(
          "grouped",
          [AccountType.Checking, AccountType.Savings, AccountType.Investment, AccountType.Loan]
            .map((type) => ({
              type,
              accounts: accounts
                .filter((a) => a.type === type)
                .map((account) => ({
                  id: account.id,
                  name: account.name,
                  type: account.type,
                  balance: 0,
                  availableBalance: 0,
                  accountNumber: "•••• •••• 9012",
                  routingNumber: "021000021",
                  interestRate: 4.65,
                  changePercent: 0.8,
                  currency: account.currency.code,
                  status: account.status,
                  openedDate: account.createdAt,
                })),
            }))
            .filter((g) => g.accounts.length > 0),
        );
      });

    const accounts = await api.accounts.list();
    await db.collection("accounts").insert(accounts);
  }
}

type Grouped = {
  type: AccountType;
  accounts: Account[];
}[];

export type Account = {
  id: string;
  name: string;
  type: AccountType;
  balance: number;
  availableBalance?: number;
  accountNumber: string;
  routingNumber?: string;
  interestRate?: number;
  changePercent?: number;
  currency: string;
  status: "active" | "frozen" | "closed";
  openedDate: string;
};
