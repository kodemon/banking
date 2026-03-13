import { createFileRoute } from "@tanstack/react-router";
import {
  ArrowDownLeft,
  ArrowUpDown,
  ArrowUpRight,
  MoreHorizontal,
  Plus,
  Search,
  SlidersHorizontal,
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
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { cn } from "@/libraries/utils";

export const Route = createFileRoute("/_user/transactions")({
  component: RouteComponent,
});

// --- Types ---
type TransactionType = "credit" | "debit";
type TransactionCategory = "transfer" | "payment" | "deposit" | "withdrawal" | "fee";

interface Transaction {
  id: string;
  description: string;
  merchant: string;
  amount: number;
  type: TransactionType;
  category: TransactionCategory;
  date: string;
  account: string;
  status: "completed" | "pending" | "failed";
}

// --- Seed data ---
const SEED: Transaction[] = [
  {
    id: "txn_001",
    description: "Direct Deposit",
    merchant: "Employer Inc.",
    amount: 3200.0,
    type: "credit",
    category: "deposit",
    date: "2026-03-06",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_002",
    description: "Rent Payment",
    merchant: "City Properties LLC",
    amount: 1450.0,
    type: "debit",
    category: "payment",
    date: "2026-03-05",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_003",
    description: "Grocery Run",
    merchant: "Whole Foods Market",
    amount: 87.34,
    type: "debit",
    category: "payment",
    date: "2026-03-04",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_004",
    description: "Savings Transfer",
    merchant: "Internal Transfer",
    amount: 500.0,
    type: "debit",
    category: "transfer",
    date: "2026-03-03",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_005",
    description: "Savings Transfer",
    merchant: "Internal Transfer",
    amount: 500.0,
    type: "credit",
    category: "transfer",
    date: "2026-03-03",
    account: "Savings ••9012",
    status: "completed",
  },
  {
    id: "txn_006",
    description: "Netflix Subscription",
    merchant: "Netflix",
    amount: 15.99,
    type: "debit",
    category: "payment",
    date: "2026-03-02",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_007",
    description: "ATM Withdrawal",
    merchant: "ATM #2841",
    amount: 200.0,
    type: "debit",
    category: "withdrawal",
    date: "2026-03-01",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_008",
    description: "Wire Transfer Fee",
    merchant: "Arcadia Bank",
    amount: 12.5,
    type: "debit",
    category: "fee",
    date: "2026-02-28",
    account: "Checking ••4821",
    status: "completed",
  },
  {
    id: "txn_009",
    description: "Freelance Payment",
    merchant: "Client XYZ",
    amount: 750.0,
    type: "credit",
    category: "deposit",
    date: "2026-02-27",
    account: "Checking ••4821",
    status: "pending",
  },
  {
    id: "txn_010",
    description: "Electric Bill",
    merchant: "City Power Co.",
    amount: 94.2,
    type: "debit",
    category: "payment",
    date: "2026-02-26",
    account: "Checking ••4821",
    status: "completed",
  },
];

const CATEGORY_LABELS: Record<TransactionCategory, string> = {
  transfer: "Transfer",
  payment: "Payment",
  deposit: "Deposit",
  withdrawal: "Withdrawal",
  fee: "Fee",
};

const STATUS_STYLES = {
  completed:
    "bg-emerald-50 text-emerald-700 border-emerald-200 dark:bg-emerald-950 dark:text-emerald-400 dark:border-emerald-800",
  pending: "bg-amber-50 text-amber-700 border-amber-200 dark:bg-amber-950 dark:text-amber-400 dark:border-amber-800",
  failed: "bg-red-50 text-red-700 border-red-200 dark:bg-red-950 dark:text-red-400 dark:border-red-800",
};

function formatCurrency(amount: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(amount);
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

// --- New Transaction Dialog ---
const EMPTY_FORM = {
  description: "",
  merchant: "",
  amount: "",
  type: "debit" as TransactionType,
  category: "payment" as TransactionCategory,
  account: "Checking ••4821",
};

function NewTransactionDialog({ onAdd }: { onAdd: (t: Transaction) => void }) {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY_FORM);

  function handleSubmit() {
    if (!form.description || !form.amount || !form.merchant) return;
    const newTxn: Transaction = {
      id: `txn_${Date.now()}`,
      description: form.description,
      merchant: form.merchant,
      amount: parseFloat(form.amount),
      type: form.type,
      category: form.category,
      date: new Date().toISOString().split("T")[0],
      account: form.account,
      status: "pending",
    };
    onAdd(newTxn);
    setForm(EMPTY_FORM);
    setOpen(false);
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button size="sm" className="gap-2">
          <Plus className="h-4 w-4" />
          New Transaction
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>New Transaction</DialogTitle>
          <DialogDescription>Manually record a transaction on your account.</DialogDescription>
        </DialogHeader>

        <div className="grid gap-4 py-2">
          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1.5">
              <Label>Type</Label>
              <Select value={form.type} onValueChange={(v) => setForm((f) => ({ ...f, type: v as TransactionType }))}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="debit">Debit</SelectItem>
                  <SelectItem value="credit">Credit</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-1.5">
              <Label>Category</Label>
              <Select
                value={form.category}
                onValueChange={(v) => setForm((f) => ({ ...f, category: v as TransactionCategory }))}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {Object.entries(CATEGORY_LABELS).map(([k, v]) => (
                    <SelectItem key={k} value={k}>
                      {v}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-1.5">
            <Label>Description</Label>
            <Input
              placeholder="e.g. Monthly subscription"
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            />
          </div>

          <div className="space-y-1.5">
            <Label>Merchant / Source</Label>
            <Input
              placeholder="e.g. Netflix"
              value={form.merchant}
              onChange={(e) => setForm((f) => ({ ...f, merchant: e.target.value }))}
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div className="space-y-1.5">
              <Label>Amount (USD)</Label>
              <Input
                type="number"
                min="0"
                step="0.01"
                placeholder="0.00"
                value={form.amount}
                onChange={(e) => setForm((f) => ({ ...f, amount: e.target.value }))}
              />
            </div>
            <div className="space-y-1.5">
              <Label>Account</Label>
              <Select value={form.account} onValueChange={(v) => setForm((f) => ({ ...f, account: v }))}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Checking ••4821">Checking ••4821</SelectItem>
                  <SelectItem value="Savings ••9012">Savings ••9012</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => setOpen(false)}>
            Cancel
          </Button>
          <Button onClick={handleSubmit}>Record Transaction</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

// --- Main Component ---
function RouteComponent() {
  const [transactions, setTransactions] = useState<Transaction[]>(SEED);
  const [search, setSearch] = useState("");
  const [filterType, setFilterType] = useState<"all" | TransactionType>("all");
  const [filterStatus, setFilterStatus] = useState<"all" | Transaction["status"]>("all");
  const [sortDir, setSortDir] = useState<"desc" | "asc">("desc");

  const filtered = transactions
    .filter((t) => {
      const matchSearch =
        t.description.toLowerCase().includes(search.toLowerCase()) ||
        t.merchant.toLowerCase().includes(search.toLowerCase());
      const matchType = filterType === "all" || t.type === filterType;
      const matchStatus = filterStatus === "all" || t.status === filterStatus;
      return matchSearch && matchType && matchStatus;
    })
    .sort((a, b) => {
      const diff = new Date(a.date).getTime() - new Date(b.date).getTime();
      return sortDir === "desc" ? -diff : diff;
    });

  const totalCredit = filtered.filter((t) => t.type === "credit").reduce((s, t) => s + t.amount, 0);
  const totalDebit = filtered.filter((t) => t.type === "debit").reduce((s, t) => s + t.amount, 0);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight">Transactions</h2>
          <p className="text-sm text-muted-foreground mt-0.5">
            {filtered.length} transaction{filtered.length !== 1 ? "s" : ""}
          </p>
        </div>
        <NewTransactionDialog onAdd={(t) => setTransactions((prev) => [t, ...prev])} />
      </div>

      {/* Summary cards */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
        <div className="rounded-lg border bg-card p-4">
          <p className="text-xs font-medium text-muted-foreground uppercase tracking-wider mb-1">Money In</p>
          <p className="text-xl font-semibold text-emerald-600">+{formatCurrency(totalCredit)}</p>
        </div>
        <div className="rounded-lg border bg-card p-4">
          <p className="text-xs font-medium text-muted-foreground uppercase tracking-wider mb-1">Money Out</p>
          <p className="text-xl font-semibold text-red-500">-{formatCurrency(totalDebit)}</p>
        </div>
        <div className="rounded-lg border bg-card p-4 col-span-2 sm:col-span-1">
          <p className="text-xs font-medium text-muted-foreground uppercase tracking-wider mb-1">Net</p>
          <p
            className={cn("text-xl font-semibold", totalCredit - totalDebit >= 0 ? "text-emerald-600" : "text-red-500")}
          >
            {formatCurrency(totalCredit - totalDebit)}
          </p>
        </div>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap gap-2 items-center">
        <div className="relative flex-1 min-w-[180px] max-w-xs">
          <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground" />
          <Input
            placeholder="Search transactions..."
            className="pl-8 h-9 text-sm"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>

        <Select value={filterType} onValueChange={(v) => setFilterType(v as typeof filterType)}>
          <SelectTrigger className="h-9 w-[120px] text-sm">
            <SlidersHorizontal className="h-3.5 w-3.5 mr-2 text-muted-foreground" />
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All types</SelectItem>
            <SelectItem value="credit">Credit</SelectItem>
            <SelectItem value="debit">Debit</SelectItem>
          </SelectContent>
        </Select>

        <Select value={filterStatus} onValueChange={(v) => setFilterStatus(v as typeof filterStatus)}>
          <SelectTrigger className="h-9 w-[130px] text-sm">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value="completed">Completed</SelectItem>
            <SelectItem value="pending">Pending</SelectItem>
            <SelectItem value="failed">Failed</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Table */}
      <div className="rounded-lg border bg-card overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow className="hover:bg-transparent">
              <TableHead className="w-[40px]" />
              <TableHead>Description</TableHead>
              <TableHead className="hidden sm:table-cell">Account</TableHead>
              <TableHead className="hidden md:table-cell">Category</TableHead>
              <TableHead className="hidden md:table-cell">Status</TableHead>
              <TableHead>
                <Button
                  variant="ghost"
                  size="sm"
                  className="-ml-3 h-8 gap-1 font-medium text-xs"
                  onClick={() => setSortDir((d) => (d === "desc" ? "asc" : "desc"))}
                >
                  Date
                  <ArrowUpDown className="h-3 w-3" />
                </Button>
              </TableHead>
              <TableHead className="text-right">Amount</TableHead>
              <TableHead className="w-[40px]" />
            </TableRow>
          </TableHeader>
          <TableBody>
            {filtered.length === 0 ? (
              <TableRow>
                <TableCell colSpan={8} className="text-center text-muted-foreground py-12 text-sm">
                  No transactions found.
                </TableCell>
              </TableRow>
            ) : (
              filtered.map((txn) => (
                <TableRow key={txn.id} className="group">
                  <TableCell className="pr-0">
                    <div
                      className={cn(
                        "flex h-7 w-7 items-center justify-center rounded-full",
                        txn.type === "credit"
                          ? "bg-emerald-50 text-emerald-600 dark:bg-emerald-950"
                          : "bg-red-50 text-red-500 dark:bg-red-950",
                      )}
                    >
                      {txn.type === "credit" ? (
                        <ArrowDownLeft className="h-3.5 w-3.5" />
                      ) : (
                        <ArrowUpRight className="h-3.5 w-3.5" />
                      )}
                    </div>
                  </TableCell>
                  <TableCell>
                    <p className="font-medium text-sm leading-tight">{txn.description}</p>
                    <p className="text-xs text-muted-foreground">{txn.merchant}</p>
                  </TableCell>
                  <TableCell className="hidden sm:table-cell text-sm text-muted-foreground">{txn.account}</TableCell>
                  <TableCell className="hidden md:table-cell">
                    <span className="text-xs text-muted-foreground">{CATEGORY_LABELS[txn.category]}</span>
                  </TableCell>
                  <TableCell className="hidden md:table-cell">
                    <Badge variant="outline" className={cn("text-[11px] capitalize", STATUS_STYLES[txn.status])}>
                      {txn.status}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-sm text-muted-foreground whitespace-nowrap">
                    {formatDate(txn.date)}
                  </TableCell>
                  <TableCell className="text-right">
                    <span
                      className={cn(
                        "font-semibold text-sm tabular-nums",
                        txn.type === "credit" ? "text-emerald-600" : "text-foreground",
                      )}
                    >
                      {txn.type === "credit" ? "+" : "-"}
                      {formatCurrency(txn.amount)}
                    </span>
                  </TableCell>
                  <TableCell>
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
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem>View details</DropdownMenuItem>
                        <DropdownMenuItem>Download receipt</DropdownMenuItem>
                        <DropdownMenuItem className="text-destructive focus:text-destructive">
                          Dispute transaction
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
