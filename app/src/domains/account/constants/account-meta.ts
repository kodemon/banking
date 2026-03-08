import { Building2, LineChart, PiggyBank, Wallet } from "lucide-react";

import { AccountType } from "../enums/account-type";

export const ACCOUNT_META: Record<AccountType, { icon: React.ElementType; label: string; color: string; bg: string }> =
  {
    [AccountType.Checking]: {
      icon: Wallet,
      label: "Checking",
      color: "text-blue-600",
      bg: "bg-blue-50 dark:bg-blue-950",
    },
    [AccountType.Savings]: {
      icon: PiggyBank,
      label: "Savings",
      color: "text-emerald-600",
      bg: "bg-emerald-50 dark:bg-emerald-950",
    },
    [AccountType.Investment]: {
      icon: LineChart,
      label: "Investment",
      color: "text-violet-600",
      bg: "bg-violet-50 dark:bg-violet-950",
    },
    [AccountType.Loan]: {
      icon: Building2,
      label: "Loan",
      color: "text-orange-600",
      bg: "bg-orange-50 dark:bg-orange-950",
    },
  };
