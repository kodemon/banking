import { createRouter } from "@tanstack/react-router";

import { session } from "@/services/session.ts";

import { routeTree } from "../routeTree.gen.ts";

export const router = createRouter({
  routeTree,
  defaultPreload: "intent",
  scrollRestoration: true,
  context: {
    session,
  },
});

export function getRouteParam(key: string): string {
  return getRouteParams()[key] ?? "";
}

export function getRouteParams(): Record<string, string> {
  return router.state.matches.at(-1)?.params ?? {};
}

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}
