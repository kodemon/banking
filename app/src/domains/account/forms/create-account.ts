import z from "zod";

import { AccountTypeSchema } from "../enums/account-type";

export const CreateAccountFormSchema = {
  name: z.string(),
  type: AccountTypeSchema,
};

export type CreateAccountForm = typeof CreateAccountFormSchema;
