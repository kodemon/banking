import { createFileRoute } from "@tanstack/react-router";
import { ArrowRight, Fingerprint } from "lucide-react";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { LoginController } from "@/domains/auth/controllers/login.controller";
import { makeControllerComponent } from "@/libraries/controller";

export const Route = createFileRoute("/login")({
  validateSearch: z.object({ return_to: z.string().optional() }),
  component: LoginPage,
});

function LoginPage() {
  const { return_to } = Route.useSearch();
  return <LoginView returnTo={return_to} />;
}

const LoginView = makeControllerComponent(LoginController, ({ loginName, processing, error, setLoginName, login }) => (
  <AuthLayout title="Welcome back" description="Enter your login name to continue with your passkey.">
    <div className="space-y-2">
      <Label htmlFor="login-name">Login name</Label>
      <Input
        id="login-name"
        type="text"
        placeholder="you@example.com"
        autoComplete="username webauthn"
        autoFocus
        value={loginName}
        onChange={(e) => setLoginName(e.target.value)}
        onKeyDown={(e) => e.key === "Enter" && login()}
        aria-invalid={!!error}
      />
    </div>

    {error && <FieldError message={error} />}

    <Button className="w-full" onClick={login} disabled={processing}>
      {processing ? (
        <>
          <Fingerprint className="w-4 h-4 animate-pulse" />
          Waiting for passkey…
        </>
      ) : (
        <>
          Continue
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

/*
 |--------------------------------------------------------------------------------
 | Shared — AuthLayout and FieldError are used by register.tsx too
 |--------------------------------------------------------------------------------
 */

export function AuthLayout({
  title,
  description,
  children,
}: {
  title: string;
  description: string;
  children: React.ReactNode;
}) {
  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-4">
      <div className="w-full max-w-sm space-y-8 animate-in fade-in slide-in-from-bottom-3 duration-300">
        <div className="space-y-1">
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">{title}</h1>
          <p className="text-sm text-muted-foreground">{description}</p>
        </div>
        <div className="space-y-4">{children}</div>
      </div>
    </div>
  );
}

export function FieldError({ message }: { message: string }) {
  return (
    <p className="text-sm text-destructive" role="alert">
      {message}
    </p>
  );
}
