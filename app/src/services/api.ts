import { payment } from "@module/payment/client";
import { HttpAdapter, makeClient, type RelayResponse } from "@platform/relay";

export const api = makeClient(
  {
    adapter: new HttpAdapter({
      url: window.location.origin,
    }),
  },
  {
    payment,
  },
);

export async function getSuccessResponse<TResponse extends RelayResponse>(
  promise: Promise<TResponse>,
): Promise<Awaited<TResponse> extends RelayResponse<infer TData> ? TData : never> {
  const response = await promise;
  if ("error" in response) {
    throw response.error;
  }
  return response.data as any;
}
