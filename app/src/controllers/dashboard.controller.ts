import type { Beneficiary } from "@module/payment/client";

import { loadBeneficiaries, payment } from "@/database/payment.ts";
import { Controller } from "@/lib/controller.tsx";
import { auth } from "@/services/auth.ts";

export class DashboardController extends Controller<{
  beneficiaries: Beneficiary[];
}> {
  #subscriptions: any[] = [];

  async onInit() {
    await this.#subscribe();
    return {
      isCreating: false,
      beneficiaries: [],
    };
  }

  async onDestroy(): Promise<void> {
    for (const subscription of this.#subscriptions) {
      subscription.unsubscribe();
    }
  }

  async createUser() {}

  async #subscribe() {
    await loadBeneficiaries();
    this.#subscriptions.push(
      payment
        .collection("beneficiary")
        .subscribe({ tenantId: auth.user.tenant.id }, { sort: { label: 1 } }, (documents) => {
          this.setState("beneficiaries", documents);
        }),
    );
  }
}
