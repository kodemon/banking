import { createFileRoute } from "@tanstack/react-router";

import { AccountList } from "@/domains/account/components/accounts-list";
import { CreateAccountDialog } from "@/domains/account/components/create-account-dialog";

export const Route = createFileRoute("/_user/")({
  component: RouteComponent,
});

function formatCurrency(amount: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 2,
  }).format(Math.abs(amount));
}

function RouteComponent() {
  const totalAssets = 0;
  const totalLiabilities = 0;
  const netWorth = totalAssets - totalLiabilities;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight">Accounts</h2>
          <p className="text-sm text-muted-foreground mt-0.5">4 accounts</p>
        </div>
        <CreateAccountDialog />
      </div>

      {/* Net worth summary */}
      <div className="grid grid-cols-3 gap-3">
        <div className="rounded-lg border border-emerald-200 bg-emerald-50 dark:bg-emerald-950 dark:border-emerald-800 p-4">
          <p className="text-xs font-medium text-emerald-700 dark:text-emerald-400 uppercase tracking-wider mb-1">
            Total Assets
          </p>
          <p className="text-xl font-semibold text-emerald-700 dark:text-emerald-300">{formatCurrency(totalAssets)}</p>
        </div>
        <div className="rounded-lg border border-orange-200 bg-orange-50 dark:bg-orange-950 dark:border-orange-800 p-4">
          <p className="text-xs font-medium text-orange-700 dark:text-orange-400 uppercase tracking-wider mb-1">
            Liabilities
          </p>
          <p className="text-xl font-semibold text-orange-700 dark:text-orange-300">
            -{formatCurrency(totalLiabilities)}
          </p>
        </div>
        <div className="rounded-lg border border-blue-200 bg-blue-50 dark:bg-blue-950 dark:border-blue-800 p-4">
          <p className="text-xs font-medium text-blue-700 dark:text-blue-400 uppercase tracking-wider mb-1">
            Net Worth
          </p>
          <p className="text-xl font-semibold text-blue-700 dark:text-blue-300">{formatCurrency(netWorth)}</p>
        </div>
      </div>

      <AccountList />
    </div>
  );
}
