import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";

import type { Session } from "@/services/session";

export const Route = createRootRouteWithContext<{
  session: Session;
}>()({
  beforeLoad: async ({ context: { session } }) => {
    await session.resolve();
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
