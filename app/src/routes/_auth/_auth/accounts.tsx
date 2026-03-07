import { createFileRoute } from "@tanstack/react-router";
import {
  ArrowLeftRight,
  Building2,
  Check,
  Copy,
  Eye,
  EyeOff,
  LineChart,
  MoreHorizontal,
  PiggyBank,
  Plus,
  TrendingDown,
  TrendingUp,
  Wallet,
} from "lucide-react";
import { useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/_auth/_auth/accounts")({
  component: RouteComponent,
});

// --- Types ---
type AccountType = "checking" | "savings" | "investment" | "loan";

interface Account {
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
}

// --- Seed data ---
const SEED: Account[] = [
  {
    id: "acc_001",
    name: "Primary Checking",
    type: "checking",
    balance: 12481.55,
    availableBalance: 11981.55,
    accountNumber: "•••• •••• 4821",
    routingNumber: "021000021",
    changePercent: 2.4,
    currency: "USD",
    status: "active",
    openedDate: "2019-03-12",
  },
  {
    id: "acc_002",
    name: "High-Yield Savings",
    type: "savings",
    balance: 28750.0,
    availableBalance: 28750.0,
    accountNumber: "•••• •••• 9012",
    routingNumber: "021000021",
    interestRate: 4.65,
    changePercent: 0.8,
    currency: "USD",
    status: "active",
    openedDate: "2020-07-01",
  },
  {
    id: "acc_003",
    name: "Investment Portfolio",
    type: "investment",
    balance: 54320.88,
    accountNumber: "•••• •••• 3344",
    interestRate: undefined,
    changePercent: -1.2,
    currency: "USD",
    status: "active",
    openedDate: "2021-01-15",
  },
  {
    id: "acc_004",
    name: "Auto Loan",
    type: "loan",
    balance: -8200.0,
    accountNumber: "•••• •••• 7756",
    interestRate: 5.9,
    changePercent: undefined,
    currency: "USD",
    status: "active",
    openedDate: "2023-06-20",
  },
];

const ACCOUNT_META: Record<AccountType, { icon: React.ElementType; label: string; color: string; bg: string }> = {
  checking: {
    icon: Wallet,
    label: "Checking",
    color: "text-blue-600",
    bg: "bg-blue-50 dark:bg-blue-950",
  },
  savings: {
    icon: PiggyBank,
    label: "Savings",
    color: "text-emerald-600",
    bg: "bg-emerald-50 dark:bg-emerald-950",
  },
  investment: {
    icon: LineChart,
    label: "Investment",
    color: "text-violet-600",
    bg: "bg-violet-50 dark:bg-violet-950",
  },
  loan: {
    icon: Building2,
    label: "Loan",
    color: "text-orange-600",
    bg: "bg-orange-50 dark:bg-orange-950",
  },
};

function formatCurrency(amount: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 2,
  }).format(Math.abs(amount));
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString("en-US", {
    month: "long",
    year: "numeric",
  });
}

// --- Copy button ---
function CopyButton({ value }: { value: string }) {
  const [copied, setCopied] = useState(false);
  function handleCopy() {
    navigator.clipboard.writeText(value);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  }
  return (
    <button onClick={handleCopy} className="ml-1 text-muted-foreground hover:text-foreground transition-colors">
      {copied ? <Check className="h-3 w-3" /> : <Copy className="h-3 w-3" />}
    </button>
  );
}

// --- Account Card ---
function AccountCard({ account }: { account: Account }) {
  const [showNumber, setShowNumber] = useState(false);
  const meta = ACCOUNT_META[account.type];
  const Icon = meta.icon;
  const isLoan = account.type === "loan";
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
          <button onClick={() => setShowNumber((s) => !s)} className="hover:text-foreground transition-colors">
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

// --- New Account Dialog ---
const EMPTY_FORM = {
  name: "",
  type: "checking" as AccountType,
};

function NewAccountDialog({ onAdd }: { onAdd: (a: Account) => void }) {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY_FORM);

  function handleSubmit() {
    if (!form.name) return;
    const newAccount: Account = {
      id: `acc_${Date.now()}`,
      name: form.name,
      type: form.type,
      balance: 0,
      availableBalance: form.type !== "investment" && form.type !== "loan" ? 0 : undefined,
      accountNumber: `•••• •••• ${Math.floor(1000 + Math.random() * 9000)}`,
      routingNumber: "021000021",
      interestRate: form.type === "savings" ? 4.65 : form.type === "loan" ? 5.9 : undefined,
      currency: "USD",
      status: "active",
      openedDate: new Date().toISOString().split("T")[0],
    };
    onAdd(newAccount);
    setForm(EMPTY_FORM);
    setOpen(false);
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button size="sm" className="gap-2">
          <Plus className="h-4 w-4" />
          Open Account
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>Open New Account</DialogTitle>
          <DialogDescription>Choose an account type to get started.</DialogDescription>
        </DialogHeader>

        <div className="grid gap-4 py-2">
          <div className="space-y-1.5">
            <Label>Account type</Label>
            <Select value={form.type} onValueChange={(v) => setForm((f) => ({ ...f, type: v as AccountType }))}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(ACCOUNT_META).map(([k, v]) => (
                  <SelectItem key={k} value={k}>
                    {v.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-1.5">
            <Label>Account nickname</Label>
            <Input
              placeholder={`e.g. ${ACCOUNT_META[form.type].label} Account`}
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            />
          </div>

          {/* Type description */}
          <div className="rounded-lg bg-muted/50 border p-3 text-xs text-muted-foreground leading-relaxed">
            {form.type === "checking" && "For everyday spending with a debit card and unlimited transactions."}
            {form.type === "savings" && "Earn 4.65% APY with no monthly fees. Ideal for building an emergency fund."}
            {form.type === "investment" && "Self-directed brokerage account. Invest in stocks, ETFs, and more."}
            {form.type === "loan" && "Apply for a personal or auto loan at competitive rates."}
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => setOpen(false)}>
            Cancel
          </Button>
          <Button onClick={handleSubmit} disabled={!form.name}>
            Open Account
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

// --- Main ---
function RouteComponent() {
  const [accounts, setAccounts] = useState<Account[]>(SEED);

  const totalAssets = accounts.filter((a) => a.balance > 0).reduce((s, a) => s + a.balance, 0);
  const totalLiabilities = accounts.filter((a) => a.balance < 0).reduce((s, a) => s + Math.abs(a.balance), 0);
  const netWorth = totalAssets - totalLiabilities;

  const grouped = (["checking", "savings", "investment", "loan"] as AccountType[])
    .map((type) => ({
      type,
      accounts: accounts.filter((a) => a.type === type),
    }))
    .filter((g) => g.accounts.length > 0);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight">Accounts</h2>
          <p className="text-sm text-muted-foreground mt-0.5">
            {accounts.length} account{accounts.length !== 1 ? "s" : ""}
          </p>
        </div>
        <NewAccountDialog onAdd={(a) => setAccounts((prev) => [...prev, a])} />
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
    </div>
  );
}
