import { Loader2 } from 'lucide-react';

interface WaitingProps {
  message: string;
}

export function Waiting({ message }: WaitingProps) {
  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <div className="flex flex-col items-center gap-4">
        <Loader2 className="h-10 w-10 animate-spin text-primary" />
        <span className="text-lg text-muted-foreground">{message}</span>
      </div>
    </section>
  );
}