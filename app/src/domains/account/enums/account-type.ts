import z from "zod";

export const AccountType = {
  Checking: 0,
  Savings: 1,
  Loan: 2,
  Investment: 3,
} as const;

export const AccountTypeSchema = z.enum(AccountType);

export function getAccounTypeLabel(type: AccountType): keyof typeof AccountType {
  switch (type) {
    case AccountType.Checking: {
      return "Checking";
    }
    case AccountType.Savings: {
      return "Savings";
    }
    case AccountType.Loan: {
      return "Loan";
    }
    case AccountType.Investment: {
      return "Investment";
    }
  }
}

export type AccountType = z.infer<typeof AccountTypeSchema>;
