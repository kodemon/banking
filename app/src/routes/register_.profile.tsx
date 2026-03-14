import { createFileRoute, redirect } from "@tanstack/react-router";
import { format } from "date-fns";
import {  CalendarIcon, ChevronRight, Loader2 } from "lucide-react";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { AuthLayout } from "@/domains/auth/components/layout";
import { ProfileController } from "@/domains/auth/controllers/register-profile.controller";
import { makeControllerComponent } from "@/libraries/controller";
import { cn } from "@/libraries/utils";
import { FieldError } from "@/components/field-error";

const ProfileView = makeControllerComponent(
  ProfileController,
  ({ details, errors, processing, error, setField, submit }) => {
    const [dobOpen, setDobOpen] = useState(false);
    return (
      <AuthLayout
        title="Complete your profile"
        description="Tell us a bit about yourself to finish setting up your account."
      >
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

          {error && <FieldError message={error} />}

          <Button type="button" className="w-full" onClick={submit} disabled={processing}>
            {processing ? (
              <>
                <Loader2 className="w-4 h-4 animate-spin" />
                Saving…
              </>
            ) : (
              <>
                Continue
                <ChevronRight className="w-4 h-4" />
              </>
            )}
          </Button>
        </div>
      </AuthLayout>
    );
  },
);

export const Route = createFileRoute("/register_/profile")({
  beforeLoad: async ({ context: { session } }) => {
    if (session.isGuest === true) {
      throw redirect({ to: "/login" });
    }
    if (session.isRegistered === true) {
      throw redirect({ to: "/" });
    }
  },
  component: ProfileView,
});
