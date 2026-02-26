import { Controller } from "@/lib/controller";
import { Form } from "@/lib/form";
import { api } from "@/services/api";

import { type PrincipalForm, PrincipalFormSchema } from "../forms/create-principal.form";

export class CreatePrincipalController extends Controller<{
  isProcessing: boolean;
  form: Form<PrincipalForm>;
  dob?: Date;
  errors: Record<string, string>;
}> {
  async onInit() {
    const principal = await this.#getPrincipal();
    return {
      isProcessing: false,
      form: new Form(PrincipalFormSchema)
        .onSubmit(async (data) => {
          await api.POST("/api/Users", {
            body: {
              name: {
                given: data.givenName,
                family: data.familyName,
              },
              email: data.email,
              dateOfBirth: data.dateOfBirth.toUTCString(),
            },
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

  async #getPrincipal() {
    // ...
  }
}
