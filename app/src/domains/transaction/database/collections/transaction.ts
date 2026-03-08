import type { Registrars } from "@valkyr/db";
import z from "zod";

import { CurrencySchema } from "@/value-objects/currency";

import { TransactionStatusSchema } from "../../enums/transaction-status";
import { TransactionTypeSchema } from "../../enums/transaction-type";

export const TransactionSchema = z.strictObject({
  id: z.guid(),
  type: TransactionTypeSchema,
  status: TransactionStatusSchema,
  referenceNumber: z.string(),
  description: z.string(),
  amount: z.number(),
  currency: CurrencySchema,
  createdAt: z.date(),
});

export type Transaction = z.infer<typeof TransactionSchema>;

export default {
  name: "transactions",
  schema: TransactionSchema.shape,
  indexes: [
    {
      field: "id",
      kind: "primary",
    },
  ],
} satisfies Registrars;
