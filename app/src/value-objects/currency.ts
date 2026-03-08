import z from "zod";

export const CurrencySchema = z.union([
  z.literal("NOK"),
  z.literal("USD"),
  z.literal("EUR"),
  z.literal("GBP"),
  z.literal("SEK"),
  z.literal("DKK"),
]);

export const Currency = {
  NOK: "NOK",
  USD: "USD",
  EUR: "EUR",
  GBP: "GBP",
  SEK: "SEK",
  DKK: "DKK",

  fromCode(code: string): Currency {
    return CurrencySchema.parse(code);
  },

  equals(a: Currency, b: Currency): boolean {
    return a === b;
  },
} as const;

export type Currency = z.infer<typeof CurrencySchema>;
