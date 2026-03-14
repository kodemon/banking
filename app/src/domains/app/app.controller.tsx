import { Controller } from "@/libraries/controller";
import { api } from "@/services/api";

export class AppController extends Controller<{
  hasUser: boolean;
}> {
  async onInit() {
    const user = await api.users.me();
    return {
      hasUser: user !== undefined,
    };
  }
}
