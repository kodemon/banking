import { format } from "date-fns";
import { AlertCircle, CalendarIcon, ChevronRight, Loader2, Shield } from "lucide-react";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { makeControllerComponent } from "@/lib/controller";
import { cn } from "@/lib/utils";

import { CreatePrincipalController } from "../controllers/create-principal.controller";

export const CreatePrincipal = makeControllerComponent(CreatePrincipalController, ({ form, errors, isProcessing }) => {
  const givenNameError = errors.givenName;
  const familyNameError = errors.familyName;
  const emailError = errors.email;
  const dobError = errors.dateOfBirth;

  const [dobOpen, setDobOpen] = useState(false);
  const [dobValue, setDobValue] = useState<Date | undefined>(undefined);

  return (
    <div className="min-h-screen bg-stone-50 flex" style={{ fontFamily: "'Georgia', serif" }}>
      {/* Left panel */}
      <div className="hidden lg:flex w-105 shrink-0 bg-stone-900 flex-col justify-between p-12 relative overflow-hidden">
        <div
          className="absolute inset-0 opacity-[0.04]"
          style={{
            backgroundImage:
              "linear-gradient(rgba(255,255,255,0.3) 1px, transparent 1px), linear-gradient(90deg, rgba(255,255,255,0.3) 1px, transparent 1px)",
            backgroundSize: "40px 40px",
          }}
        />

        <div className="relative z-10">
          <div className="flex items-center gap-2.5 mb-16">
            <div className="w-8 h-8 rounded bg-blue-400 flex items-center justify-center">
              <Shield className="w-4 h-4 text-stone-900" />
            </div>
            <span className="text-white font-semibold tracking-wide text-lg">Valkyr Bank</span>
          </div>

          <h1 className="text-white text-4xl font-semibold leading-tight tracking-tight mb-6">
            Set up your
            <br />
            account profile.
          </h1>
          <p className="text-stone-400 text-sm leading-relaxed">
            To get started, we just need a few personal details to create your secure banking principal.
          </p>
        </div>

        <div className="relative z-10 space-y-5">
          {[
            { label: "Secure & encrypted", desc: "Your data is protected end-to-end" },
            { label: "Regulatory compliant", desc: "Meets all KYC requirements" },
            { label: "Instant access", desc: "Start banking immediately after setup" },
          ].map((item) => (
            <div key={item.label} className="flex items-start gap-3">
              <div className="mt-0.5 w-4 h-4 rounded-full border border-amber-400/60 flex items-center justify-center shrink-0">
                <div className="w-1.5 h-1.5 rounded-full bg-amber-400" />
              </div>
              <div>
                <p className="text-stone-200 text-sm font-medium">{item.label}</p>
                <p className="text-stone-500 text-xs mt-0.5">{item.desc}</p>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Right panel */}
      <div className="flex-1 flex flex-col">
        <div className="flex-1 flex items-center justify-center px-8">
          <div className="w-full max-w-md">
            <form onSubmit={form.submit} className="space-y-8 animate-in fade-in slide-in-from-bottom-3 duration-400">
              <div>
                <h2 className="text-2xl font-semibold text-stone-800 tracking-tight">Create your personal account</h2>
                <p className="text-stone-500 text-sm mt-2">
                  Enter your basic details to create your personal principal.
                </p>
              </div>

              <div className="space-y-5">
                <div className="grid grid-cols-2 gap-4">
                  {/* First name */}
                  <div className="space-y-1.5">
                    <Label
                      className="text-xs font-medium text-stone-600 uppercase tracking-wide"
                      style={{ fontFamily: "system-ui, sans-serif" }}
                    >
                      First name
                    </Label>
                    <Input
                      placeholder="Jane"
                      {...form.register("givenName")}
                      aria-invalid={!!givenNameError}
                      className={`bg-white text-stone-800 placeholder:text-stone-300 transition-colors
                          ${
                            givenNameError
                              ? "border-red-300 focus:border-red-500 focus:ring-red-500"
                              : "border-stone-200 focus:border-stone-400 focus:ring-stone-400"
                          }`}
                      style={{ fontFamily: "system-ui, sans-serif" }}
                    />
                    <FieldError error={givenNameError} />
                  </div>

                  {/* Last name */}
                  <div className="space-y-1.5">
                    <Label
                      className="text-xs font-medium text-stone-600 uppercase tracking-wide"
                      style={{ fontFamily: "system-ui, sans-serif" }}
                    >
                      Last name
                    </Label>
                    <Input
                      placeholder="Doe"
                      {...form.register("familyName")}
                      aria-invalid={!!familyNameError}
                      className={`bg-white text-stone-800 placeholder:text-stone-300 transition-colors
                          ${
                            familyNameError
                              ? "border-red-300 focus:border-red-500 focus:ring-red-500"
                              : "border-stone-200 focus:border-stone-400 focus:ring-stone-400"
                          }`}
                      style={{ fontFamily: "system-ui, sans-serif" }}
                    />
                    <FieldError error={familyNameError} />
                  </div>
                </div>

                {/* Email */}
                <div className="space-y-1.5">
                  <Label
                    className="text-xs font-medium text-stone-600 uppercase tracking-wide"
                    style={{ fontFamily: "system-ui, sans-serif" }}
                  >
                    Email address
                  </Label>
                  <Input
                    type="email"
                    placeholder="jane.doe@example.com"
                    {...form.register("email")}
                    aria-invalid={!!emailError}
                    className={`bg-white text-stone-800 placeholder:text-stone-300 transition-colors
                        ${
                          emailError
                            ? "border-red-300 focus:border-red-500 focus:ring-red-500"
                            : "border-stone-200 focus:border-stone-400 focus:ring-stone-400"
                        }`}
                    style={{ fontFamily: "system-ui, sans-serif" }}
                  />
                  {!emailError && (
                    <p className="text-xs text-stone-400" style={{ fontFamily: "system-ui, sans-serif" }}>
                      We'll use this to contact you and secure your account.
                    </p>
                  )}
                  <FieldError error={emailError} />
                </div>

                {/* Date of birth */}
                <div className="space-y-1.5">
                  <Label
                    className="text-xs font-medium text-stone-600 uppercase tracking-wide"
                    style={{ fontFamily: "system-ui, sans-serif" }}
                  >
                    Date of birth
                  </Label>
                  <Popover open={dobOpen} onOpenChange={setDobOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        type="button"
                        variant="outline"
                        aria-invalid={!!dobError}
                        className={cn(
                          "w-full h-9 justify-start text-left font-normal bg-white transition-colors",
                          !dobValue && "text-stone-300",
                          dobError
                            ? "border-red-300 hover:border-red-400 focus:border-red-500 focus:ring-red-500"
                            : "border-stone-200 hover:border-stone-300 focus:border-stone-400 focus:ring-stone-400",
                        )}
                        style={{ fontFamily: "system-ui, sans-serif" }}
                      >
                        <CalendarIcon className="mr-2 h-4 w-4 shrink-0 text-stone-400" />
                        {dobValue ? (
                          <span className="text-stone-800">{format(dobValue, "MMMM d, yyyy")}</span>
                        ) : (
                          <span>Select your date of birth</span>
                        )}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                      <Calendar
                        mode="single"
                        selected={dobValue}
                        onSelect={(date) => {
                          setDobValue(date);
                          // Register the value with the form controller
                          if (date !== undefined) {
                            form.set("dateOfBirth", date);
                          }
                          setDobOpen(false);
                        }}
                        // Allow navigation back to reasonable birth year range
                        captionLayout="dropdown"
                        startMonth={new Date(1960, 0)}
                        endMonth={new Date(new Date().getFullYear() - 18, 0)}
                        defaultMonth={dobValue ?? new Date(new Date().getFullYear() - 30, 0)}
                      />
                    </PopoverContent>
                  </Popover>
                  {!dobError && (
                    <p className="text-xs text-stone-400" style={{ fontFamily: "system-ui, sans-serif" }}>
                      Required for identity verification. You must be 18 or older.
                    </p>
                  )}
                  <FieldError error={dobError} />
                </div>
              </div>

              <div className="pt-2">
                <Button
                  type="submit"
                  disabled={isProcessing}
                  className="w-full bg-stone-900 hover:bg-stone-800 text-white h-11 text-sm font-medium tracking-wide transition-all duration-200"
                  style={{ fontFamily: "system-ui, sans-serif" }}
                >
                  {isProcessing ? (
                    <Loader2 className="w-4 h-4 animate-spin" />
                  ) : (
                    <>
                      Create account
                      <ChevronRight className="w-4 h-4 ml-1" />
                    </>
                  )}
                </Button>

                <p className="text-center text-xs text-stone-400 mt-4" style={{ fontFamily: "system-ui, sans-serif" }}>
                  By continuing you agree to our{" "}
                  <a href="#" className="underline underline-offset-2 hover:text-stone-600 transition-colors">
                    Terms of Service
                  </a>{" "}
                  and{" "}
                  <a href="#" className="underline underline-offset-2 hover:text-stone-600 transition-colors">
                    Privacy Policy
                  </a>
                  .
                </p>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
});

function FieldError({ error }: { error?: unknown }) {
  if (!error) {
    return null;
  }
  const message =
    typeof error === "string"
      ? error
      : typeof error === "object" && error !== null && "message" in error
        ? String((error as any).message)
        : "Invalid value";
  return (
    <div
      className="flex items-start gap-1.5 text-xs text-red-600 mt-1 animate-in fade-in"
      style={{ fontFamily: "system-ui, sans-serif" }}
    >
      <AlertCircle className="w-3.5 h-3.5 mt-px shrink-0" />
      <span className="leading-tight">{message}</span>
    </div>
  );
}
