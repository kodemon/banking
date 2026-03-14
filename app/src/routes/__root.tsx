import { createRootRouteWithContext, Outlet, redirect } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";

import type { Session } from "@/services/session";

const PUBLIC_ACCESS = ["/login", "/register"];

export const Route = createRootRouteWithContext<{
  session: Session;
}>()({
  beforeLoad: async ({ context: { session }, location }) => {
    if (PUBLIC_ACCESS.includes(location.pathname)) {
      return;
    }
    const authenticated = await session.isAuthenticated();
    if (authenticated === false) {
      throw redirect({
        to: "/login",
        search: {
          return_to: location.pathname,
        },
      });
    }
  },
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
