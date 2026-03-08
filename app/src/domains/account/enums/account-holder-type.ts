import z from "zod";

export const AccountHolderType = {
  Primary: 0,
  Beneficiary: 1,
  Guardian: 2,
  PowerOfAttorney: 3,
  Custodian: 4,
  Operating: 5,
  Trust: 6,
  Escrow: 7,
  Investment: 8,
  Payroll: 9,
} as const;

export const AccountHolderTypeSchema = z.enum(AccountHolderType);

export function getAccountStatusLabel(type: AccountHolderType): keyof typeof AccountHolderType {
  switch (type) {
    case AccountHolderType.Primary: {
      return "Primary";
    }
    case AccountHolderType.Beneficiary: {
      return "Beneficiary";
    }
    case AccountHolderType.Guardian: {
      return "Guardian";
    }
    case AccountHolderType.PowerOfAttorney: {
      return "PowerOfAttorney";
    }
    case AccountHolderType.Custodian: {
      return "Custodian";
    }
    case AccountHolderType.Operating: {
      return "Operating";
    }
    case AccountHolderType.Trust: {
      return "Trust";
    }
    case AccountHolderType.Escrow: {
      return "Escrow";
    }
    case AccountHolderType.Investment: {
      return "Investment";
    }
    case AccountHolderType.Payroll: {
      return "Payroll";
    }
  }
}

export type AccountHolderType = z.infer<typeof AccountHolderTypeSchema>;
