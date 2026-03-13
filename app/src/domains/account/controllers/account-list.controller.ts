import { Controller } from "@/libraries/controller";
import { session } from "@/services/session";

import { db, resolveAccounts } from "../database/dbcontext";
import type { AccountStatus } from "../enums/account-status";
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
    await resolveAccounts();
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
                  accountNumber: {
                    private: `•••• •• ${account.number.slice(-6)}`,
                    public: account.number,
                  },
                  routingNumber: "021000021",
                  interestRate: 4.65,
                  changePercent: 0.8,
                  currency: account.currency,
                  status: account.status,
                  openedDate: account.createdAt,
                })),
            }))
            .filter((g) => g.accounts.length > 0),
        );
      });
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
  accountNumber: {
    private: string;
    public: string;
  };
  routingNumber?: string;
  interestRate?: number;
  changePercent?: number;
  currency: string;
  status: AccountStatus;
  openedDate: Date;
};
