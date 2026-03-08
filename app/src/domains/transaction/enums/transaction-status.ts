import z from "zod";

export const TransactionStatus = {
  Pending: 0,
  Failed: 1,
  Reversed: 2,
  Completed: 3,
} as const;

export const TransactionStatusSchema = z.enum(TransactionStatus);
