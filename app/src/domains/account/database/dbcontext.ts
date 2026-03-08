import { IndexedDB, type Registrars } from "@valkyr/db";

import { accountRegistrar } from "./collections/account";

export const db = new IndexedDB({
  name: "valkyr-db:banking:accounts",
  registrars: [accountRegistrar] satisfies Registrars[],
});
