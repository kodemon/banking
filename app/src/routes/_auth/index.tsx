import { createFileRoute } from "@tanstack/react-router";

function DashboardComponent() {
  return <div>Dashboard</div>;
}

export const Route = createFileRoute("/_auth/")({
  component: DashboardComponent,
});
