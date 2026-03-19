import { createFileRoute, redirect } from "@tanstack/react-router";
import { ChevronRight, Loader2 } from "lucide-react";

import { FieldError } from "@/components/field-error";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { AuthLayout } from "@/domains/auth/components/layout";
import { RegistrationController } from "@/domains/auth/controllers/registration.controller";
import { makeControllerComponent } from "@/libraries/controller";

const RegisterView = makeControllerComponent(
  RegistrationController,
  ({ email, emailError, processing, error, setEmail, setupPasskey }) => (
    <AuthLayout
      title="Create your account"
      description="Enter your email, then set up a passkey to sign in with Face ID, Touch ID, or your device PIN."
    >
      <div className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="email">Email address</Label>
          <Input
            id="email"
            type="email"
            placeholder="you@example.com"
            autoFocus
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && setupPasskey()}
            aria-invalid={!!emailError}
          />
          {emailError && <FieldError message={emailError} />}
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

        <p className="text-center text-xs text-muted-foreground">
          Already have an account?{" "}
          <a href="/login" className="underline underline-offset-2 hover:text-foreground transition-colors">
            Sign in
          </a>
        </p>
      </div>
    </AuthLayout>
  ),
);

export const Route = createFileRoute("/register")({
  beforeLoad: async ({ context: { session } }) => {
    if (session.isRegistered === true) {
      throw redirect({ to: "/" });
    }
    if (session.isAuthenticated === true) {
      throw redirect({ to: "/register/profile" });
    }
  },
  component: RegisterView,
});
