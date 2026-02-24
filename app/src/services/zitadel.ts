import { createZitadelAuth, type ZitadelConfig } from "@zitadel/react";

const config: ZitadelConfig = {
  authority: "https://iam.valkyrjs.com",
  project_resource_id: "356672856689475587",
  client_id: "356672963459743747",
  redirect_uri: "http://localhost:5173/callback",
  post_logout_redirect_uri: "http://localhost:5173",
  response_type: "code",
  scope: "openid profile email urn:zitadel:iam:user:metadata urn:zitadel:iam:org:id:354958981547950083",
};

export const zitadel = createZitadelAuth(config);

export type ZitadelUser = NonNullable<Awaited<ReturnType<typeof zitadel.userManager.getUser>>>;
