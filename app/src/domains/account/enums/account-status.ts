import z from "zod";

export const AccountStatus = {
  Active: 0,
  Frozen: 1,
  Closed: 2,
} as const;

export const AccountStatusSchema = z.enum(AccountStatus);

export function getAccountStatusLabel(status: AccountStatus): keyof typeof AccountStatus {
  switch (status) {
    case AccountStatus.Active: {
      return "Active";
    }
    case AccountStatus.Frozen: {
      return "Frozen";
    }
    case AccountStatus.Closed: {
      return "Closed";
    }
  }
}

export type AccountStatus = z.infer<typeof AccountStatusSchema>;
