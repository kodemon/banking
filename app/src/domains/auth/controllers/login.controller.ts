import { Controller } from "@/libraries/controller";
import { router } from "@/libraries/router";
import { api } from "@/services/api";

import { isPublicKeyCredential, prepareRequestOptions, serializeAssertion } from "../libraries/webauthn";

export class LoginController extends Controller<
  {
    processing: boolean;
    error: string;
  },
  {
    returnTo?: string;
  }
> {
  async onInit() {
    return {
      processing: false,
      error: "",
    };
  }

  async login() {
    this.setState({ processing: true, error: "" });

    try {
      // 1. Begin login — use a random key to correlate the challenge with the verify call.
      //    The new API no longer needs a username; it's a discoverable credential flow.
      const sessionKey = crypto.randomUUID();

      const { options } = await api.auth.login.passkey.begin({ sessionKey });

      // 2. Prompt the browser to select a passkey.
      const credential = await navigator.credentials.get({
        publicKey: prepareRequestOptions(options as Record<string, unknown>),
      });

      if (!isPublicKeyCredential(credential)) {
        throw new Error("Passkey authentication was cancelled or is not supported.");
      }

      // 3. Verify the assertion — server validates and sets the session cookie.
      await api.auth.login.passkey.verify({
        sessionKey,
        assertionResponse: serializeAssertion(credential),
      });

      router.navigate({ to: this.props.returnTo ?? "/" });
    } catch (err) {
      const message =
        err instanceof DOMException && err.name === "NotAllowedError"
          ? "Passkey prompt was dismissed. Press Continue to try again."
          : err instanceof Error
            ? err.message
            : "Something went wrong.";
      this.setState("error", message);
    } finally {
      this.setState("processing", false);
    }
  }
}
