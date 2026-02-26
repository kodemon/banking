import { Controller } from "@/lib/controller";

export class AppController extends Controller<{
  hasPrincipal: boolean;
  hasUser: boolean;
}> {
  async onInit() {
    return {
      hasPrincipal: false,
      hasUser: false,
    };
  }
}
