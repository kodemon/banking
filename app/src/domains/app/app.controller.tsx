import { Controller } from "@/lib/controller";
import { api } from "@/services/api";

export class AppController extends Controller<{
  hasUser: boolean;
}> {
  async onInit() {
    let hasUser = false;

    const principal = await api.GET("/api/principals/me");
    if ("data" in principal) {
      hasUser = principal.data?.attributes.user?.userId !== null;
    }

    return {
      hasUser,
    };
  }
}
