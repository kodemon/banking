import { createFileRoute } from "@tanstack/react-router";
import { format } from "date-fns";
import { AlertCircle, ArrowLeft, CalendarIcon, ChevronRight, Loader2 } from "lucide-react";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { RegistrationController } from "@/domains/auth/controllers/registration.controller";
import type { DetailsForm } from "@/domains/auth/schemas/details-form";
import { makeControllerComponent } from "@/libraries/controller";
import { cn } from "@/libraries/utils";

import { AuthLayout } from "./login";

const RouteComponent = makeControllerComponent(
  RegistrationController,
  ({ step, details, errors, processing, error, setField, submitDetails, backToDetails, setupPasskey }) => {
    if (step === "passkey") {
      return (
        <AuthLayout
          title="Set up your passkey"
          description="Your passkey lets you sign in with Face ID, Touch ID, or your device PIN — no password needed."
        >
          <div className="space-y-4">
            <div className="rounded-lg border bg-muted/40 px-4 py-3 text-sm space-y-1">
              <p className="font-medium text-foreground">
                {details.givenName} {details.familyName}
              </p>
              <p className="text-muted-foreground">{details.email}</p>
            </div>

            {error && <FieldError message={error} />}

            <Button className="w-full" onClick={setupPasskey} disabled={processing}>
              {processing ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin" />
                  Setting up…
                </>
              ) : (
                <>
                  Set up passkey
                  <ChevronRight className="w-4 h-4" />
                </>
              )}
            </Button>

            <button
              type="button"
              onClick={backToDetails}
              className="flex items-center gap-1.5 text-xs text-muted-foreground hover:text-foreground transition-colors mx-auto"
            >
              <ArrowLeft className="w-3 h-3" />
              Back
            </button>
          </div>
        </AuthLayout>
      );
    }

    return (
      <AuthLayout title="Create your account" description="Enter your details to get started.">
        <DetailsStep details={details} errors={errors} setField={setField} onSubmit={submitDetails} />
      </AuthLayout>
    );
  },
);

export const Route = createFileRoute("/register")({
  component: RouteComponent,
});

/*
 |--------------------------------------------------------------------------------
 | Details step
 |--------------------------------------------------------------------------------
 */

function DetailsStep({
  details,
  errors,
  setField,
  onSubmit,
}: {
  details: Partial<DetailsForm>;
  errors: Partial<Record<keyof DetailsForm, string>>;
  setField: <K extends keyof DetailsForm>(key: K, value: DetailsForm[K]) => void;
  onSubmit: () => void;
}) {
  const [dobOpen, setDobOpen] = useState(false);

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-3">
        <div className="space-y-2">
          <Label htmlFor="given-name">First name</Label>
          <Input
            id="given-name"
            placeholder="Jane"
            autoFocus
            value={details.givenName ?? ""}
            onChange={(e) => setField("givenName", e.target.value)}
            aria-invalid={!!errors.givenName}
          />
          <FieldError message={errors.givenName} />
        </div>

        <div className="space-y-2">
          <Label htmlFor="family-name">Last name</Label>
          <Input
            id="family-name"
            placeholder="Doe"
            value={details.familyName ?? ""}
            onChange={(e) => setField("familyName", e.target.value)}
            aria-invalid={!!errors.familyName}
          />
          <FieldError message={errors.familyName} />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="email">Email address</Label>
        <Input
          id="email"
          type="email"
          placeholder="jane.doe@example.com"
          value={details.email ?? ""}
          onChange={(e) => setField("email", e.target.value)}
          aria-invalid={!!errors.email}
        />
        <FieldError message={errors.email} />
      </div>

      <div className="space-y-2">
        <Label>Date of birth</Label>
        <Popover open={dobOpen} onOpenChange={setDobOpen}>
          <PopoverTrigger asChild>
            <Button
              type="button"
              variant="outline"
              aria-invalid={!!errors.dateOfBirth}
              className={cn(
                "w-full justify-start text-left font-normal",
                !details.dateOfBirth && "text-muted-foreground",
                !!errors.dateOfBirth && "border-destructive",
              )}
            >
              <CalendarIcon className="mr-2 h-4 w-4" />
              {details.dateOfBirth ? format(details.dateOfBirth, "MMMM d, yyyy") : "Select date of birth"}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start">
            <Calendar
              mode="single"
              selected={details.dateOfBirth}
              onSelect={(date) => {
                if (date) setField("dateOfBirth", date);
                setDobOpen(false);
              }}
              captionLayout="dropdown"
              startMonth={new Date(1960, 0)}
              endMonth={new Date(new Date().getFullYear() - 18, 0)}
              defaultMonth={details.dateOfBirth ?? new Date(new Date().getFullYear() - 30, 0)}
            />
          </PopoverContent>
        </Popover>
        <FieldError message={errors.dateOfBirth} />
        {!errors.dateOfBirth && <p className="text-xs text-muted-foreground">You must be 18 or older.</p>}
      </div>

      <Button type="button" className="w-full" onClick={onSubmit}>
        Continue
        <ChevronRight className="w-4 h-4" />
      </Button>

      <p className="text-center text-xs text-muted-foreground">
        Already have an account?{" "}
        <a href="/login" className="underline underline-offset-2 hover:text-foreground transition-colors">
          Sign in
        </a>
      </p>
    </div>
  );
}

/*
 |--------------------------------------------------------------------------------
 | Shared
 |--------------------------------------------------------------------------------
 */

function FieldError({ message }: { message?: string }) {
  if (!message) return null;
  return (
    <div className="flex items-start gap-1.5 text-xs text-destructive animate-in fade-in">
      <AlertCircle className="w-3.5 h-3.5 mt-px shrink-0" />
      <span>{message}</span>
    </div>
  );
}
