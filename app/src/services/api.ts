import createClient from "openapi-fetch";

import { ApiError } from "@/libraries/errors";
import type { components, paths } from "@/openapi.gen";

const client = createClient<paths>({ baseUrl: "/" });

export const api = {
  auth: {
    register: async (body: components["schemas"]["RegisterRequest"]) => {
      const response = await client.POST("/api/auth/register", { body });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },

    passkeyRegistrationBegin: async (body: components["schemas"]["PasskeyRegistrationBeginRequest"]) => {
      const response = await client.POST("/api/auth/passkey/registration/begin", { body });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },

    passkeyRegistrationVerify: async (body: components["schemas"]["PasskeyRegistrationVerifyRequest"]) => {
      const response = await client.POST("/api/auth/passkey/registration/verify", {
        body,
        credentials: "include",
      });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
    },

    passkeyLoginBegin: async (body: components["schemas"]["PasskeyBeginRequest"]) => {
      const response = await client.POST("/api/auth/passkey/begin", {
        body,
        credentials: "include",
      });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },

    passkeyLoginVerify: async (body: components["schemas"]["PasskeyVerifyRequest"]) => {
      const response = await client.POST("/api/auth/passkey/verify", {
        body,
        credentials: "include",
      });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
    },

    logout: async () => {
      await client.POST("/api/auth/logout", { credentials: "include" });
    },

    session: async () => {
      const response = await client.GET("/api/auth/session", { credentials: "include" });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },
  },

  accounts: {
    create: async (body: components["schemas"]["CreateAccountPayload"]) => {
      const response = await client.POST("/api/accounts", { body });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },

    list: async () => {
      const response = await client.GET("/api/accounts");
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },
  },

  principals: {
    me: async () => {
      const response = await client.GET("/api/principals/me");
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },
  },

  users: {
    create: async (body: components["schemas"]["PostUserPayload"]) => {
      const response = await client.POST("/api/users", { body });
      if (response.error !== undefined) {
        throw new ApiError(response.error);
      }
      return response.data;
    },

    me: async () => {
      const response = await client.GET("/api/users/me");
      if (response.error !== undefined) {
        if (response.error.status === 404) {
          return undefined;
        }
        throw new ApiError(response.error);
      }
      return response.data;
    },
  },
};
