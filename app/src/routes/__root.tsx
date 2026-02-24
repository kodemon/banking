import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";

import type { AuthContext } from "@/services/auth.ts";

export const Route = createRootRouteWithContext<{
  auth: AuthContext;
}>()({
  component: RootComponent,
});

function RootComponent() {
  return (
    <>
      <Outlet />
      <TanStackRouterDevtools position="bottom-right" initialIsOpen={false} />
    </>
  );
}
