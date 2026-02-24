import { IndexedDB } from "@valkyr/db";
import type { Registrars } from "node_modules/@valkyr/db/_dist/src/registrars";

import userRegistrar from "./collections/user";

export const users = new IndexedDB({
  name: "valkyr-db:banking",
  registrars: [userRegistrar] satisfies Registrars[],
});
