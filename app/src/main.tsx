import { RouterProvider } from "@tanstack/react-router";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";

import { ThemeProvider } from "@/components/theme-provider.tsx";
import { router } from "@/lib/router";

import "./index.css";

const rootElement = document.getElementById("root");
if (rootElement === null) {
  throw new Error("Failed to retrieve root element");
}

createRoot(rootElement).render(
  <StrictMode>
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <RouterProvider router={router} />
    </ThemeProvider>
  </StrictMode>,
);
