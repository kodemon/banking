import { createFileRoute } from "@tanstack/react-router";

import { DashboardController } from "@/controllers/dashboard.controller.ts";
import { makeControllerComponent } from "@/lib/controller.tsx";

const DashboardComponent = makeControllerComponent(DashboardController, ({ beneficiaries }) => {
  if (beneficiaries.length === 0) {
    return (
      <div className="w-full flex flex-col pt-20 items-center justify-center text-center">
        <h1 className="text-2xl font-semibold tracking-tight">No Beneficiaries Found</h1>
        <p className="text-muted-foreground mt-2 max-w-sm">
          This tenant does not have a beneficiaries assigned. A beneficiary entity is required to continue.
        </p>
      </div>
    );
  }
  return (
    <div>
      Payments
      <pre>{JSON.stringify(beneficiaries, null, 2)}</pre>
    </div>
  );
});

export const Route = createFileRoute("/_auth/")({
  component: DashboardComponent,
});
