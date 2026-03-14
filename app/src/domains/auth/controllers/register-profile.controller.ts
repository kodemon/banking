import { Controller } from "@/libraries/controller";
import { router } from "@/libraries/router";
import { api } from "@/services/api";

import { type DetailsForm, DetailsFormSchema } from "../schemas/details-form";

export class ProfileController extends Controller<{
  details: Partial<DetailsForm>;
  errors: Partial<Record<keyof DetailsForm, string>>;
  processing: boolean;
  error: string;
}> {
  async onInit() {
    return {
      details: { givenName: "", familyName: "", email: "", dateOfBirth: undefined },
      errors: {},
      processing: false,
      error: "",
    };
  }

  setField<K extends keyof DetailsForm>(key: K, value: DetailsForm[K]) {
    this.setState("details", { ...this.state.details, [key]: value });
    if (this.state.errors[key]) {
      this.setState("errors", { ...this.state.errors, [key]: undefined });
    }
  }

  async submit() {
    const result = DetailsFormSchema.safeParse(this.state.details);
    if (!result.success) {
      const errors: Partial<Record<keyof DetailsForm, string>> = {};
      for (const issue of result.error.issues) {
        const key = issue.path[0] as keyof DetailsForm;
        if (!errors[key]) errors[key] = issue.message;
      }
      this.setState("errors", errors);
      return;
    }

    this.setState({ processing: true, error: "" });

    try {
      const details = result.data;
      await api.users.create({
        email: details.email,
        name: { given: details.givenName, family: details.familyName },
        dateOfBirth: details.dateOfBirth.toISOString(),
      });
      router.navigate({ to: "/" });
    } catch (err) {
      this.setState({
        error: err instanceof Error ? err.message : "Something went wrong. Please try again.",
      });
    } finally {
      this.setState("processing", false);
    }
  }
}
