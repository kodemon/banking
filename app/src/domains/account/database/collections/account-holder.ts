import z from "zod";

import { AccountHolderTypeSchema } from "../../enums/account-holder-type";

export const AccountHolderSchema = z.strictObject({
  id: z.guid(),
  accountId: z.guid(),
  holderId: z.guid(),
  type: AccountHolderTypeSchema,
  createdAt: z.coerce.date(),
});

export type AccountHolder = z.infer<typeof AccountHolderSchema>;
