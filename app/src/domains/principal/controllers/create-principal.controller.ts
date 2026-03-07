import { Controller } from "@/lib/controller";
import { Form } from "@/lib/form";
import { router } from "@/lib/router";
import { api } from "@/services/api";

import { type PrincipalForm, PrincipalFormSchema } from "../forms/create-principal.form";

export class CreatePrincipalController extends Controller<{
  isProcessing: boolean;
  form: Form<PrincipalForm>;
  dob?: Date;
  errors: Record<string, string>;
}> {
  async onInit() {
    await this.#resolvePrincipalState();
    return {
      isProcessing: false,
      form: new Form(PrincipalFormSchema)
        .onSubmit(async (data) => {
          await api
            .POST("/api/users", {
              body: {
                name: {
                  given: data.givenName,
                  family: data.familyName,
                },
                email: data.email,
                dateOfBirth: data.dateOfBirth.toISOString(),
              },
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

  async #resolvePrincipalState() {
    const { error, data } = await api.GET("/api/principals/me");
    if (error) {
      throw error;
    }
    if (data.attributes.user?.userId !== null) {
      router.navigate({ to: "/" });
    }
  }
}
