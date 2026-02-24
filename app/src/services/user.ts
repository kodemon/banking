import z from "zod";

import { type ZitadelUser, zitadel } from "./zitadel.ts";

let cached: User | undefined;

const TenantSchema = z.strictObject({
  id: z.string(),
  name: z.string(),
  domain: z.string(),
});

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

  readonly tenant: Tenant;

  constructor({ id, name, email, avatar }: Profile, tenant: Tenant) {
    this.id = id;
    this.name = name;
    this.email = email;
    this.avatar = avatar;
    this.tenant = tenant;
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

  static get tenantId() {
    return User.instance.tenant.id;
  }

  static get tenantName() {
    return User.instance.tenant.name;
  }

  static async resolve() {
    if (cached === undefined) {
      const user = await zitadel.userManager.getUser();
      if (user === null) {
        return undefined;
      }
      cached = new User(
        getUserProfile(user),
        TenantSchema.parse({
          id: user.profile["urn:zitadel:iam:org:id"],
          name: user.profile["urn:zitadel:iam:user:resourceowner:name"],
          domain: user.profile["urn:zitadel:iam:user:resourceowner:primary_domain"],
        }),
      );
    }
    return cached;
  }
}

function getUserProfile({ profile }: ZitadelUser): Profile {
  const user: Profile = {
    id: profile.client_id as string,
    name: "Unknown",
    email: "unknown@acme.none",
    avatar: "",
  };
  if (profile.name) {
    user.name = profile.name;
  } else if (profile.given_name && profile.family_name) {
    user.name = `${profile.given_name} ${profile.family_name}`;
  } else if (profile.given_name) {
    user.name = profile.given_name;
  }
  if (profile.email) {
    user.email = profile.email;
  }
  if (profile.picture !== undefined) {
    user.avatar = profile.picture;
  }
  return ProfileSchema.parse(user);
}

type Tenant = z.output<typeof TenantSchema>;
type Profile = z.output<typeof ProfileSchema>;
