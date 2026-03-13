import { Controller } from "@/libraries/controller";
import { api } from "@/services/api";

export class AppController extends Controller<{
  hasUser: boolean;
}> {
  async onInit() {
    let hasUser = false;

    const result = await api.GET("/api/users/me");
    if ("data" in result) {
      hasUser = true;
    }

    return {
      hasUser,
    };
  }
}
