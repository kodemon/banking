import type { Registrars } from "@valkyr/db";
import z from "zod";

import { CurrencySchema } from "@/value-objects/currency";

import { AccountStatusSchema } from "../../enums/account-status";
import { AccountTypeSchema } from "../../enums/account-type";
import { AccountHolderSchema } from "./account-holder";

export const AccountSchema = z.strictObject({
  id: z.guid(),
  name: z.string(),
  type: AccountTypeSchema,
  status: AccountStatusSchema,
  currency: CurrencySchema,
  holders: z.array(AccountHolderSchema),
  createdAt: z.coerce.date(),
});

export type Account = z.infer<typeof AccountSchema>;

export const accountRegistrar = {
  name: "accounts",
  schema: AccountSchema.shape,
  indexes: [
    {
      field: "id",
      kind: "primary",
    },
  ],
} satisfies Registrars;
