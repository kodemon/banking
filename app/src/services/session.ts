import { api } from "./api";

let cachedPrincipal: Promise<{ id: string; roles: string[] }> | undefined;

export const SessionState = {
  Guest: 0,
  Authenticated: 1,
  Registered: 2,
} as const;

type SessionState = (typeof SessionState)[keyof typeof SessionState];

let sessionState: SessionState = SessionState.Guest;
let isResolved: boolean = false;

export const session = {
  async resolve() {
    if (isResolved === true) {
      return;
    }
    isResolved = true;
    const session = await api.auth.session();
    if (session === undefined) {
      return;
    }
    sessionState = SessionState.Authenticated;
    const user = await api.users.me();
    if (user === undefined) {
      return;
    }
    sessionState = SessionState.Registered;
  },

  get isGuest(): boolean {
    return sessionState === SessionState.Guest;
  },

  get isAuthenticated(): boolean {
    return sessionState !== SessionState.Guest;
  },

  get isRegistered(): boolean {
    return sessionState === SessionState.Registered;
  },

  get principal(): Promise<{ id: string; roles: string[] }> {
    if (cachedPrincipal === undefined) {
      cachedPrincipal = api.principals.me();
    }
    return cachedPrincipal;
  },

  async getPrincipalId(): Promise<string> {
    return this.principal.then((p) => p.id);
  },

  login(): void {
    window.location.href = "/login";
  },

  logout(): void {
    api.auth.logout().finally(() => {
      cachedPrincipal = undefined;
      window.location.href = "/login";
    });
  },
};

export type Session = typeof session;
