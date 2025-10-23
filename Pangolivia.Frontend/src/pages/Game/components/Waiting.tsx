import { Spinner } from "@/components/ui/spinner";

interface WaitingProps {
  message: string;
}

export function Waiting({ message }: WaitingProps) {
  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16" role="status" aria-live="polite">
      <div className="flex flex-col items-center gap-4">
        <Spinner className="h-10 w-10 text-primary" aria-hidden="true" />
        <span className="text-lg text-muted-foreground">{message}</span>
      </div>
    </section>
  );
}
