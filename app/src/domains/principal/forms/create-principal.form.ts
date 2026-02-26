import z from "zod";

export const PrincipalFormSchema = {
  givenName: z.string({ error: "Given name is required" }),
  familyName: z.string({ error: "Family name is required" }),
  email: z.email({ error: "Email is required" }),
  dateOfBirth: z.date({ error: "Date of birth is required" }),
};

export type PrincipalForm = typeof PrincipalFormSchema;
