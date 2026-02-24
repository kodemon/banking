import { User } from "@/services/user.ts";
import { zitadel } from "@/services/zitadel.ts";

export const auth: AuthContext = {
  get isAuthenticated() {
    return User.isAuthenticated;
  },

  get user() {
    return User.instance;
  },

  async resolve() {
    await User.resolve();
  },

  async login() {
    await zitadel.authorize();
  },

  async logout() {
    await zitadel.signout();
  },
};

export type AuthContext = {
  isAuthenticated: boolean;
  user: User;
  resolve: () => Promise<void>;
  login: () => Promise<void>;
  logout: () => Promise<void>;
};
