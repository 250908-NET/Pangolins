import { Button } from "@/components/ui/button";
import { TextEffect } from "@/components/ui/text-effect";
import { Link } from "react-router-dom";

export default function Home() {
  return (
    <main className="overflow-hidden [--color-primary-foreground:var(--color-white)] [--color-primary:var(--color-green-600)]">
      <section>
        <div className="relative mx-auto max-w-6xl px-6 pb-20 pt-32 lg:pt-48 flex justify-end">
          <div className="relative z-10 max-w-2xl text-right">
            <TextEffect
              per="line"
              preset="fade-in-blur"
              speedSegment={0.3}
              delay={0.5}
              as="p"
              className="mt-6 text-pretty text-lg"
            >
              Fuel your curiosity with AI‑driven trivia. Every session is
              unique, every challenge unexpected, and every victory sweeter than
              the last.
            </TextEffect>
          </div>
        </div>
      </section>
    </main>
  );
}

// ...existing code...
