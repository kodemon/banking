import { AlertCircle } from "lucide-react";

export function FieldError({ message }: { message?: string }) {
  if (message === undefined) {
    return null;
  }
  return (
    <div className="flex items-start gap-1.5 text-xs text-destructive animate-in fade-in">
      <AlertCircle className="w-3.5 h-3.5 mt-px shrink-0" />
      <span>{message}</span>
    </div>
  );
}
