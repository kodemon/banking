import { Plus } from "lucide-react";

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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { makeControllerComponent } from "@/lib/controller";

import { ACCOUNT_META } from "../constants/account-meta";
import { CreateAccountController } from "../controllers/create-account.controller";
import { AccountType, AccountTypeSchema } from "../enums/account-type";

export const CreateAccountDialog = makeControllerComponent(
  CreateAccountController,
  ({ open, processing, form: { name, type }, errors, setOpen, setForm, submit }) => {
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
              <Select
                value={type.toString()}
                onValueChange={(value) => {
                  setForm("type", AccountTypeSchema.parse(parseInt(value, 10)));
                }}
              >
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
                placeholder={`e.g. ${ACCOUNT_META[type].label} Account`}
                value={name}
                onChange={(e) => {
                  setForm("name", e.target.value);
                }}
              />
            </div>

            {/* Type description */}
            <div className="rounded-lg bg-muted/50 border p-3 text-xs text-muted-foreground leading-relaxed">
              {type === AccountType.Checking && "For everyday spending with a debit card and unlimited transactions."}
              {type === AccountType.Savings &&
                "Earn 4.65% APY with no monthly fees. Ideal for building an emergency fund."}
              {type === AccountType.Investment && "Self-directed brokerage account. Invest in stocks, ETFs, and more."}
              {type === AccountType.Loan && "Apply for a personal or auto loan at competitive rates."}
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)} disabled={processing}>
              Cancel
            </Button>
            <Button type="button" onClick={submit} disabled={processing}>
              Open Account
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    );
  },
);
