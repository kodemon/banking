import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import z from "zod";

import { auth } from "@/services/auth";
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
        // auth.resolve().then(() => {
        //   if (auth.isAuthenticated === true) {
        //     // navigate({ to: "/", replace: true });
        //   } else {
        //     console.error("Completed authentication callback, but failed to resolve principal!");
        //   }
        // });
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
