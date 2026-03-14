import createClient from "openapi-fetch";

import type { components, paths } from "@/openapi.gen";

const client = createClient<paths>({ baseUrl: "" });

export const api = {
  auth: {
    register: {
      passkey: {
        /**
         * Step 1 — generate a WebAuthn creation challenge.
         * POST /api/auth/passkey/registration/begin
         */
        begin: async (body: components["schemas"]["PasskeyRegistrationBeginRequest"]) => {
          const response = await client.POST("/api/auth/passkey/registration/begin", { body });
          if (response.error !== undefined) {
            throw new ApiException(response.error);
          }
          return response.data;
        },

        /**
         * Step 2 — verify the attestation; server creates principal + sets session cookie.
         * POST /api/auth/passkey/registration/verify
         */
        verify: async (body: components["schemas"]["PasskeyRegistrationVerifyRequest"]) => {
          const response = await client.POST("/api/auth/passkey/registration/verify", { body });
          if (response.error !== undefined) {
            throw new ApiException(response.error);
          }
        },
      },
    },

    login: {
      passkey: {
        /**
         * Step 1 — generate a WebAuthn assertion challenge.
         * POST /api/auth/passkey/login/begin
         */
        begin: async (body: components["schemas"]["PasskeyLoginBeginRequest"]) => {
          const response = await client.POST("/api/auth/passkey/login/begin", { body });
          if (response.error !== undefined) {
            throw new ApiException(response.error);
          }
          return response.data;
        },

        /**
         * Step 2 — verify the assertion; server sets session cookie.
         * POST /api/auth/passkey/login/verify
         */
        verify: async (body: components["schemas"]["PasskeyLoginVerifyRequest"]) => {
          const response = await client.POST("/api/auth/passkey/login/verify", { body });
          if (response.error !== undefined) {
            throw new ApiException(response.error);
          }
        },
      },
    },

    /**
     * Validate the current session.
     * GET /api/auth/session
     */
    session: async () => {
      const response = await client.GET("/api/auth/session");
      if (response.error !== undefined) {
        if (response.error.status === 401) {
          return undefined;
        }
        throw new ApiException(response.error);
      }
      return response.data;
    },

    /**
     * Destroy the current session and clear the cookie.
     * POST /api/auth/logout
     */
    logout: async () => {
      await client.POST("/api/auth/logout");
    },
  },

  accounts: {
    create: async (body: components["schemas"]["CreateAccountPayload"]) => {
      const response = await client.POST("/api/accounts", { body });
      if (response.error !== undefined) {
        throw new ApiException(response.error);
      }
      return response.data;
    },

    list: async () => {
      const response = await client.GET("/api/accounts");
      if (response.error !== undefined) {
        throw new ApiException(response.error);
      }
      return response.data;
    },
  },

  principals: {
    me: async () => {
      const response = await client.GET("/api/principals/me");
      if (response.error !== undefined) {
        throw new ApiException(response.error);
      }
      return response.data;
    },
  },

  users: {
    create: async (body: components["schemas"]["PostUserPayload"]) => {
      const response = await client.POST("/api/users", { body });
      if (response.error !== undefined) {
        throw new ApiException(response.error);
      }
      return response.data;
    },

    /**
     * Retrieve the user registered for the current authenticated session, or
     * undefined if no user has been created yet.
     */
    me: async () => {
      const response = await client.GET("/api/users/me");
      if (response.error !== undefined) {
        if (response.error.status === 404) {
          return undefined;
        }
        throw new ApiException(response.error);
      }
      return response.data;
    },
  },
};

class ApiException {
  readonly type: string;
  readonly title?: string;
  readonly status: number;
  readonly detail: string;
  readonly instance: string;

  constructor(error: ApiErrorResponse) {
    this.type = error.type ?? "API_EXCEPTION";
    this.title = error.title ?? "Unknown API Exception";
    this.status = typeof error.status === "string" ? parseInt(error.status, 10) : (error.status ?? 500);
    this.detail = error.detail ?? "";
    this.instance = error.instance ?? "";
  }
}

type ApiErrorResponse = {
  type?: string | null | undefined;
  title?: string | null | undefined;
  status?: string | number | null | undefined;
  detail?: string | null | undefined;
  instance?: string | null | undefined;
};
