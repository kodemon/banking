import { Controller } from "@/libraries/controller";
import { Form } from "@/libraries/form";
import { router } from "@/libraries/router";
import { api } from "@/services/api";

import { type PrincipalForm, PrincipalFormSchema } from "../forms/create-principal.form";

export class CreatePrincipalController extends Controller<{
  isProcessing: boolean;
  form: Form<PrincipalForm>;
  dob?: Date;
  errors: Record<string, string>;
}> {
  async onInit() {
    return {
      isProcessing: false,
      form: new Form(PrincipalFormSchema)
        .onSubmit(async (data) => {
          await api.users
            .create({
              name: {
                given: data.givenName,
                family: data.familyName,
              },
              email: data.email,
              dateOfBirth: data.dateOfBirth.toISOString(),
            })
            .then(() => {
              router.navigate({ to: "/" });
            });
        })
        .onProcessing((isProcessing) => {
          this.setState("isProcessing", isProcessing);
        })
        .onError((errors) => {
          this.setState("errors", errors);
        }),
      errors: {},
    };
  }
}
