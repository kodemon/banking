import { ArrowLeftRight, Check, Copy, Eye, EyeOff, MoreHorizontal, TrendingDown, TrendingUp } from "lucide-react";
import { useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Separator } from "@/components/ui/separator";
import { makeControllerComponent } from "@/lib/controller";
import { cn } from "@/lib/utils";

import { ACCOUNT_META } from "../constants/account-meta";
import { type Account, AccountListController } from "../controllers/account-list.controller";
import { AccountType } from "../enums/account-type";

export const AccountList = makeControllerComponent(AccountListController, ({ grouped }) => {
  return (
    <>
      {/* Grouped account cards */}
      {grouped.map(({ type, accounts: group }) => {
        const meta = ACCOUNT_META[type];
        return (
          <section key={type}>
            <div className="flex items-center gap-2 mb-3">
              <h3 className="text-sm font-semibold">{meta.label} Accounts</h3>
              <Badge variant="secondary" className="text-[10px] h-4 px-1.5">
                {group.length}
              </Badge>
            </div>
            <div className="grid gap-3">
              {group.map((account) => (
                <AccountCard key={account.id} account={account} />
              ))}
            </div>
          </section>
        );
      })}
    </>
  );
});

function AccountCard({ account }: { account: Account }) {
  const [showNumber, setShowNumber] = useState(false);
  const meta = ACCOUNT_META[account.type];
  const Icon = meta.icon;
  const isLoan = account.type === AccountType.Loan;
  const isNegativeChange = account.changePercent !== undefined && account.changePercent < 0;

  return (
    <div className="rounded-xl border bg-card overflow-hidden group transition-shadow hover:shadow-md">
      {/* Card header */}
      <div className="p-5 pb-4">
        <div className="flex items-start justify-between mb-4">
          <div className="flex items-center gap-3">
            <div className={cn("flex h-9 w-9 items-center justify-center rounded-lg", meta.bg)}>
              <Icon className={cn("h-4 w-4", meta.color)} />
            </div>
            <div>
              <p className="font-semibold text-sm leading-tight">{account.name}</p>
              <p className="text-xs text-muted-foreground">{meta.label}</p>
            </div>
          </div>
          <div className="flex items-center gap-1">
            <Badge
              variant="outline"
              className={cn(
                "text-[10px] capitalize",
                account.status === "active"
                  ? "text-emerald-600 border-emerald-200 bg-emerald-50 dark:bg-emerald-950 dark:border-emerald-800"
                  : "text-muted-foreground",
              )}
            >
              {account.status}
            </Badge>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 opacity-0 group-hover:opacity-100 transition-opacity"
                >
                  <MoreHorizontal className="h-4 w-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-44">
                <DropdownMenuItem>
                  <ArrowLeftRight className="mr-2 h-4 w-4" />
                  Transfer funds
                </DropdownMenuItem>
                <DropdownMenuItem>View statements</DropdownMenuItem>
                <DropdownMenuItem>Account details</DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="text-destructive focus:text-destructive">Close account</DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>

        {/* Balance */}
        <div>
          <p className="text-xs text-muted-foreground mb-0.5">{isLoan ? "Outstanding balance" : "Current balance"}</p>
          <p className={cn("text-2xl font-semibold tracking-tight tabular-nums", isLoan ? "text-orange-600" : "")}>
            {isLoan ? "-" : ""}
            {formatCurrency(account.balance)}
          </p>
          {account.availableBalance !== undefined && (
            <p className="text-xs text-muted-foreground mt-0.5">{formatCurrency(account.availableBalance)} available</p>
          )}
        </div>

        {/* Change indicator */}
        {account.changePercent !== undefined && (
          <div
            className={cn(
              "flex items-center gap-1 mt-2 text-xs font-medium",
              isNegativeChange ? "text-red-500" : "text-emerald-600",
            )}
          >
            {isNegativeChange ? <TrendingDown className="h-3.5 w-3.5" /> : <TrendingUp className="h-3.5 w-3.5" />}
            {isNegativeChange ? "" : "+"}
            {account.changePercent}% this month
          </div>
        )}
      </div>

      <Separator />

      {/* Card footer */}
      <div className="px-5 py-3 bg-muted/30 flex items-center justify-between gap-2 flex-wrap">
        <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
          <span className="font-mono">
            {showNumber ? account.accountNumber.replace(/•/g, "x") : account.accountNumber}
          </span>
          <button
            type="button"
            onClick={() => setShowNumber((s) => !s)}
            className="hover:text-foreground transition-colors"
          >
            {showNumber ? <EyeOff className="h-3 w-3" /> : <Eye className="h-3 w-3" />}
          </button>
          <CopyButton value={account.accountNumber} />
        </div>

        <div className="flex items-center gap-3 text-xs text-muted-foreground">
          {account.interestRate !== undefined && (
            <span className="font-medium text-foreground">{account.interestRate}% APY</span>
          )}
          <span>Opened {formatDate(account.openedDate)}</span>
        </div>
      </div>
    </div>
  );
}

function CopyButton({ value }: { value: string }) {
  const [copied, setCopied] = useState(false);
  function handleCopy() {
    navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  }
  return (
    <button
      type="button"
      onClick={handleCopy}
      className="ml-1 text-muted-foreground hover:text-foreground transition-colors"
    >
      {copied ? <Check className="h-3 w-3" /> : <Copy className="h-3 w-3" />}
    </button>
  );
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString("en-US", {
    month: "long",
    year: "numeric",
  });
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 2,
  }).format(Math.abs(amount));
}
