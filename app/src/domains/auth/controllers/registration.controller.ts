import z from "zod";

import { Controller } from "@/libraries/controller";
import { router } from "@/libraries/router";
import { api } from "@/services/api";

import {
  getPlatformName,
  isPublicKeyCredential,
  prepareCreationOptions,
  serializeAttestation,
} from "../libraries/webauthn";

const RegistrationFormSchema = z.object({
  email: z.email("Enter a valid email address."),
});

export class RegistrationController extends Controller<{
  email: string;
  emailError: string;
  processing: boolean;
  error: string;
}> {
  async onInit() {
    return {
      email: "",
      emailError: "",
      processing: false,
      error: "",
    };
  }

  setEmail(value: string) {
    this.setState({ email: value, emailError: "" });
  }

  async setupPasskey() {
    // Validate email before touching the authenticator.
    const result = RegistrationFormSchema.safeParse({ email: this.state.email });
    if (!result.success) {
      this.setState({ emailError: result.error.issues[0]?.message });
      return;
    }

    this.setState({ processing: true, error: "" });

    try {
      const { email } = result.data;

      // 1. Begin — server generates a challenge keyed to a new userId.
      const { userId, options } = await api.auth.register.passkey.begin({ email });

      // 2. Prompt the browser for a new credential.
      const publicKeyOptions = prepareCreationOptions(options as Record<string, unknown>);

      let credential: Credential | null = null;
      try {
        credential = await navigator.credentials.create({ publicKey: publicKeyOptions });
      } catch (credErr) {
        if (credErr instanceof DOMException && credErr.name === "NotAllowedError") {
          throw new Error("Passkey prompt was dismissed. Press Set up passkey to try again.");
        }
        throw new Error(`Passkey creation failed: ${credErr instanceof Error ? credErr.message : String(credErr)}`);
      }

      if (!isPublicKeyCredential(credential)) {
        throw new Error("Passkey setup was cancelled or is not supported on this device.");
      }

      // 3. Verify — server creates the principal and sets the session cookie.
      await api.auth.register.passkey.verify({
        userId,
        credentialName: getPlatformName(),
        attestationResponse: serializeAttestation(credential),
      });

      // 4. Session active — _user guard will redirect to /register/profile.
      router.navigate({ to: "/" });
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
