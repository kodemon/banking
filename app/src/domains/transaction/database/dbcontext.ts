import { IndexedDB, type Registrars } from "@valkyr/db";

import journalEntryRegistrar from "./collections/journal-entry";
import transactionRegistrar from "./collections/transaction";

export const db = new IndexedDB({
  name: "valkyr-db:banking:transaction",
  registrars: [journalEntryRegistrar, transactionRegistrar] satisfies Registrars[],
});
