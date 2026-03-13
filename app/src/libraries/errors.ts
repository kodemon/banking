export class ApiError {
  readonly type: string;
  readonly title?: string;
  readonly status: number;
  readonly detail: string;
  readonly instance: string;

  constructor(error: ApiErrorResponse) {
    this.type = error.type ?? "API_EXCEPTION";
    this.title = error.title ?? "Unknown API Exception";
    this.status = typeof error.status === "string" ? parseInt(error.status, 10) : (error.status ?? 500);
    this.detail = error.detail ?? "";
    this.instance = error.instance ?? "";
  }
}

type ApiErrorResponse = {
  type?: string | null | undefined;
  title?: string | null | undefined;
  status?: string | number | null | undefined;
  detail?: string | null | undefined;
  instance?: string | null | undefined;
};
