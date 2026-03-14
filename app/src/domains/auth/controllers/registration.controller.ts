import { Controller } from "@/libraries/controller";
import { router } from "@/libraries/router";
import { api } from "@/services/api";

import { getPlatformName, isPublicKeyCredential, prepareCreationOptions, serializeAttestation } from "../libraries/webauthn";
import { type DetailsForm, DetailsFormSchema } from "../schemas/details-form";

type Step = "details" | "passkey";
 
export class RegistrationController extends Controller<{
  step: Step;
  details: Partial<DetailsForm>;
  errors: Partial<Record<keyof DetailsForm, string>>;
  processing: boolean;
  error: string;
}> {
  async onInit() {
    return {
      step: "details" as Step,
      details: {
        givenName: "",
        familyName: "",
        email: "",
        dateOfBirth: undefined,
      },
      errors: {},
      processing: false,
      error: "",
    };
  }
 
  // -------------------------------------------------------------------------
  // Details step
  // -------------------------------------------------------------------------
 
  setField<K extends keyof DetailsForm>(key: K, value: DetailsForm[K]) {
    this.setState("details", { ...this.state.details, [key]: value });
    // Clear the error for this field as the user edits it.
    if (this.state.errors[key]) {
      this.setState("errors", { ...this.state.errors, [key]: undefined });
    }
  }
 
  submitDetails() {
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
    this.setState({ errors: {}, step: "passkey" });
  }
 
  backToDetails() {
    this.setState({ step: "details", error: "" });
  }
 
  // -------------------------------------------------------------------------
  // Passkey step
  // -------------------------------------------------------------------------
 
  async setupPasskey() {
    this.setState({ processing: true, error: "" });
 
    try {
      const details = DetailsFormSchema.parse(this.state.details);
 
      // 1. Create the Zitadel user.
      const { userId } = await api.auth.register({
        givenName: details.givenName,
        familyName: details.familyName,
        email: details.email,
      });
 
      // 2. Begin passkey registration — get the WebAuthn creation challenge.
      const { passkeyId, creationOptions } = await api.auth.passkeyRegistrationBegin({ userId });
 
      console.log("[passkey] rp.id:", (creationOptions as any)?.publicKey?.rp);
 
      // 3. Prompt the browser for a new credential.
      const publicKeyOptions = prepareCreationOptions(
        (creationOptions as Record<string, unknown>).publicKey as Record<string, unknown>,
      );
 
      let credential: Credential | null = null;
      try {
        credential = await navigator.credentials.create({
          publicKey: publicKeyOptions,
        });
      } catch (credErr) {
        if (credErr instanceof DOMException && credErr.name === "NotAllowedError") {
          throw new Error("Passkey prompt was dismissed. Press Set up passkey to try again.");
        }
        throw new Error(
          `Passkey creation failed: ${credErr instanceof Error ? credErr.message : String(credErr)}`,
        );
      }
 
      if (!isPublicKeyCredential(credential)) {
        throw new Error("Passkey setup was cancelled or is not supported on this device.");
      }
 
      // 4. Verify the credential with Zitadel — the server sets the session
      // cookie on success so the user is immediately authenticated.
      await api.auth.passkeyRegistrationVerify({
        userId,
        passkeyId,
        passkeyName: getPlatformName(),
        publicKeyCredential: serializeAttestation(credential),
      });
 
      // 5. Navigate to the banking profile setup.
      router.navigate({ to: "/register/profile" });
    } catch (err) {
      const message =
        err instanceof DOMException && err.name === "NotAllowedError"
          ? "Passkey prompt was dismissed. Press Set up passkey to try again."
          : err instanceof Error
            ? err.message
            : "Something went wrong. Please try again.";
      this.setState({ error: message });
    } finally {
      this.setState("processing", false);
    }
  }
}
