import z from "zod";

export const JournalEntryType = {
  Credit: 0,
  Debig: 1,
} as const;

export const JournalEntryTypeSchema = z.enum(JournalEntryType);
