import z from "zod";

export const DetailsFormSchema = z.object({
  givenName: z.string().min(1, "First name is required."),
  familyName: z.string().min(1, "Last name is required."),
  email: z.email("Enter a valid email address."),
  dateOfBirth: z.date({ error: "Date of birth is required." }).refine((date) => {
    const age = new Date().getFullYear() - date.getFullYear();
    return age >= 18;
  }, "You must be 18 or older."),
});

export type DetailsForm = z.output<typeof DetailsFormSchema>;
