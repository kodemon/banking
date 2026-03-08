import z from "zod";

export const TransactionType = {
  Deposit: 0,
  Withdrawal: 1,
  Transfer: 2,
  Fee: 3,
  Interest: 4,
} as const;

export const TransactionTypeSchema = z.enum(TransactionType);
