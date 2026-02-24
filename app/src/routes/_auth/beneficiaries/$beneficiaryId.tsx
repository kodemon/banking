import { createFileRoute } from "@tanstack/react-router";

import { BeneficiaryComponent } from "@/components/beneficiary/read.component.tsx";

export const Route = createFileRoute("/_auth/beneficiaries/$beneficiaryId")({
  component: BeneficiaryRoute,
});

function BeneficiaryRoute() {
  const { beneficiaryId } = Route.useParams();
  return <BeneficiaryComponent id={beneficiaryId} />;
}
