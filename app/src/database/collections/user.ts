import type { Registrars } from "node_modules/@valkyr/db/_dist/src/registrars";
import z from "zod";

export const UserSchema = z.strictObject({
  email: z.email(),
});

export default {
  name: "users",
  schema: UserSchema.shape,
  indexes: [
    {
      field: "id",
      kind: "primary",
    },
    {
      field: "email",
      kind: "unique",
    },
  ],
} satisfies Registrars;
