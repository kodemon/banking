import { createFileRoute, redirect } from "@tanstack/react-router";
import { ArrowRight, Fingerprint } from "lucide-react";
import { z } from "zod";

import { FieldError } from "@/components/field-error";
import { Button } from "@/components/ui/button";
import { AuthLayout } from "@/domains/auth/components/layout";
import { LoginController } from "@/domains/auth/controllers/login.controller";
import { makeControllerComponent } from "@/libraries/controller";

export const Route = createFileRoute("/login")({
  beforeLoad: async ({ context: { session } }) => {
    if (session.isRegistered === true) {
      throw redirect({ to: "/" });
    }
    if (session.isAuthenticated === true) {
      throw redirect({ to: "/register/profile" });
    }
  },
  validateSearch: z.object({ return_to: z.string().optional() }),
  component: LoginPage,
});

function LoginPage() {
  const { return_to } = Route.useSearch();
  return <LoginView returnTo={return_to} />;
}

const LoginView = makeControllerComponent(LoginController, ({ processing, error, login }) => (
  <AuthLayout title="Welcome back" description="Use your passkey — Face ID, Touch ID, or device PIN — to sign in.">
    {error && <FieldError message={error} />}

    <Button className="w-full" onClick={login} disabled={processing}>
      {processing ? (
        <>
          <Fingerprint className="w-4 h-4 animate-pulse" />
          Waiting for passkey…
        </>
      ) : (
        <>
          Continue with passkey
          <ArrowRight className="w-4 h-4" />
        </>
      )}
    </Button>

    <p className="text-center text-xs text-muted-foreground">
      Don't have an account?{" "}
      <a href="/register" className="underline underline-offset-2 hover:text-foreground transition-colors">
        Create one
      </a>
    </p>
  </AuthLayout>
));
