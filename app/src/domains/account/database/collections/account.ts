import type { Registrars } from "@valkyr/db";
import z from "zod";

import { CurrencySchema } from "@/value-objects/currency";

import { AccountHolderTypeSchema } from "../../enums/account-holder-type";
import { AccountStatusSchema } from "../../enums/account-status";
import { AccountTypeSchema } from "../../enums/account-type";

export const AccountSchema = z.strictObject({
  id: z.guid(),
  number: z.string(),
  name: z.string(),
  type: AccountTypeSchema,
  status: AccountStatusSchema,
  currency: CurrencySchema,
  holders: z.array(
    z.strictObject({
      id: z.guid(),
      accountId: z.guid(),
      holderId: z.guid(),
      type: AccountHolderTypeSchema,
      createdAt: z.coerce.date(),
    }),
  ),
  createdAt: z.coerce.date(),
});

export type Account = z.infer<typeof AccountSchema>;

export const accountRegistrar = {
  name: "accounts" as const,
  schema: AccountSchema.shape,
  indexes: [
    {
      field: "id",
      kind: "primary",
    },
  ],
} satisfies Registrars;
