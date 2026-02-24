import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import z from "zod";

import { zitadel } from "@/services/zitadel.ts";

export const Route = createFileRoute("/callback")({
  validateSearch: z.object({
    code: z.string(),
  }),
  component: AuthComponent,
});

function AuthComponent() {
  const navigate = useNavigate();
  const [error, setError] = useState();

  useEffect(() => {
    zitadel.userManager
      .signinRedirectCallback()
      .then(() => {
        navigate({ to: "/", replace: true });
      })
      .catch((error) => {
        console.error("Callback error", error);
        setError(error);
      });
  }, [navigate]);

  if (error) {
    return <div>Error!</div>;
  }

  return null;
}
