import type { components } from "@/openapi.gen";

import { api } from "./api";

let cachedPrincipal: Promise<Principal> | undefined;

export const session = {
  async isAuthenticated(): Promise<boolean> {
    return fetch("/api/auth/session", { credentials: "include" }).then((res) => res.ok);
  },

  get principal(): Promise<Principal> {
    if (cachedPrincipal === undefined) {
      cachedPrincipal = api.principals.me();
    }
    return cachedPrincipal;
  },

  async getPrincipalId(): Promise<string> {
    return this.principal.then((principal) => principal.id);
  },

  login(): void {
    window.location.href = `/api/auth/login?return_to=${encodeURIComponent(window.location.pathname)}`;
  },

  logout(): void {
    window.location.href = "/api/auth/logout";
  },
};

export type Session = typeof session;

export type Principal = components["schemas"]["ResolvedPrincipal"];
