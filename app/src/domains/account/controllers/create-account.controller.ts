import z from "zod";

import { Controller } from "@/libraries/controller";
import { api } from "@/services/api";
import { Currency } from "@/value-objects/currency";

import { pushAccounts } from "../database/dbcontext";
import { AccountHolderType, AccountHolderTypeSchema } from "../enums/account-holder-type";
import { AccountType, AccountTypeSchema } from "../enums/account-type";

export const CreateAccountFormSchema = z.object({
  name: z.string(),
  type: AccountTypeSchema,
  holder: AccountHolderTypeSchema,
});

type Form = z.output<typeof CreateAccountFormSchema>;

export class CreateAccountController extends Controller<{
  open: boolean;
  processing: boolean;
  form: z.output<typeof CreateAccountFormSchema>;
  errors: Record<string, string>;
}> {
  async onInit() {
    return {
      open: false,
      processing: false,
      form: {
        name: "",
        type: AccountType.Checking,
        holder: AccountHolderType.Primary,
      },
      errors: {},
    };
  }

  setOpen(state: boolean) {
    this.setState("open", state);
  }

  setForm<TKey extends keyof Form>(key: TKey, value: Form[TKey]) {
    this.setState("form", {
      ...this.state.form,
      [key]: value,
    });
  }

  reset() {
    this.setState("form", {
      name: "",
      type: AccountType.Checking,
      holder: AccountHolderType.Primary,
    });
  }

  async submit() {
    this.setState("processing", true);
    await api.accounts
      .create({
        accountName: this.state.form.name,
        accountType: this.state.form.type,
        currency: Currency.NOK,
        holderType: this.state.form.holder,
      })
      .then((account) => {
        pushAccounts([account]);
        this.setState("open", false);
        this.reset();
      })
      .catch((error) => {
        console.log(error);
      })
      .finally(() => {
        this.setState("processing", false);
      });
  }
}
