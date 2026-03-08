import z from "zod";

import { api } from "./api.ts";
import { zitadel } from "./zitadel.ts";

let cached: User | undefined;

const ProfileSchema = z.strictObject({
  id: z.string(),
  name: z.string(),
  email: z.string(),
  avatar: z.string(),
});

export class User {
  readonly id: string;

  readonly name: string;
  readonly email: string;
  readonly avatar: string;

  constructor({ id, name, email, avatar }: Profile) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.avatar = avatar;
  }

  static get isAuthenticated() {
    return cached !== undefined;
  }

  static get instance() {
    if (cached === undefined) {
      throw zitadel.authorize();
    }
    return cached;
  }

  static async resolve() {
    if (cached === undefined) {
      const user = await getUser();
      if (user === undefined) {
        return undefined;
      }
      cached = new User({
        id: user.ownerId,
        name: `${user.name.given} ${user.name.family}`.trim(),
        email: "",
        avatar: "",
      });
    }
    return cached;
  }
}

async function getUser() {
  const result = await api.GET("/api/users/me");
  if ("error" in result) {
    return undefined;
  }
  return result.data;
}

type Profile = z.output<typeof ProfileSchema>;
