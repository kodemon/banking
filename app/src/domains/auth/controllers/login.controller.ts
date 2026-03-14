import { Controller } from "@/libraries/controller";
import { router } from "@/libraries/router";
import { api } from "@/services/api";

import { isPublicKeyCredential, prepareRequestOptions, serializeAssertion } from "../libraries/webauthn";

export class LoginController extends Controller<
  {
    loginName: string;
    processing: boolean;
    error: string;
  },
  {
    returnTo?: string;
  }
> {
  async onInit() {
    return {
      loginName: "",
      processing: false,
      error: "",
    };
  }

  setLoginName(value: string) {
    this.setState({ loginName: value, error: "" });
  }

  async login() {
    if (!this.state.loginName.trim()) {
      this.setState("error", "Please enter your login name.");
      return;
    }

    this.setState({ processing: true, error: "" });

    try {
      const { sessionId, sessionToken, challengeOptions } = await api.auth.passkeyLoginBegin({
        loginName: this.state.loginName.trim(),
      });

      const credential = await navigator.credentials.get({
        publicKey: prepareRequestOptions(
          (challengeOptions as Record<string, unknown>).publicKey as Record<string, unknown>,
        ),
      });

      if (!isPublicKeyCredential(credential)) {
        throw new Error("Passkey authentication was cancelled or is not supported.");
      }

      await api.auth.passkeyLoginVerify({
        sessionId,
        sessionToken,
        credentialAssertionData: serializeAssertion(credential),
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
