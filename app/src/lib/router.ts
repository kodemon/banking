import { createRouter } from "@tanstack/react-router";

import { auth } from "@/services/auth.ts";

import { routeTree } from "../routeTree.gen.ts";

export const router = createRouter({
  routeTree,
  defaultPreload: "intent",
  scrollRestoration: true,
  context: {
    auth,
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
