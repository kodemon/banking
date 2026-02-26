import { createFileRoute, Outlet } from "@tanstack/react-router";

import { AppController } from "@/domains/app/app.controller";
import { CreatePrincipal } from "@/domains/principal/components/CreatePrincipal";
import { makeControllerComponent } from "@/lib/controller";

const AppLayout = makeControllerComponent(AppController, ({ hasPrincipal }) => {
  if (hasPrincipal === false) {
    return <CreatePrincipal />;
  }
  return (
    <div>
      <Outlet />
    </div>
  );
});

export const Route = createFileRoute("/_auth")({
  beforeLoad: async ({ context: { auth } }) => {
    await auth.resolve();
    if (auth.isAuthenticated === false) {
      throw auth.login();
    }
  },
  component: AppLayout,
});
