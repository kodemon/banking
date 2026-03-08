import type { Registrars } from "@valkyr/db";
import z from "zod";

import { JournalEntryTypeSchema } from "../../enums/journal-entry-type";

export const JournalEntrySchema = z.strictObject({
  id: z.guid(),
  transactionId: z.guid(),
  participantId: z.guid(),
  type: JournalEntryTypeSchema,
  createdAt: z.date(),
});

export type JournalEntry = z.infer<typeof JournalEntrySchema>;

export default {
  name: "journal_entries",
  schema: JournalEntrySchema.shape,
  indexes: [
    {
      field: "id",
      kind: "primary",
    },
  ],
} satisfies Registrars;
