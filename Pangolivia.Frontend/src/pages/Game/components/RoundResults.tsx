import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { CheckCircle, XCircle } from 'lucide-react';
import type { RoundResults } from '@/contexts/SignalRContext';

interface RoundResultsProps {
  results: RoundResults;
  currentUserId?: number;
}

export function RoundResults({ results, currentUserId }: RoundResultsProps) {
  const myScoreThisRound = results.playerScores.find(p => p.userId === currentUserId)?.score ?? 0;

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <Card className="w-full max-w-2xl text-center">
        <CardHeader>
          {myScoreThisRound > 0 ? (
            <CheckCircle className="mx-auto h-12 w-12 text-green-500" />
          ) : (
            <XCircle className="mx-auto h-12 w-12 text-red-500" />
          )}
          <CardTitle className="text-3xl">{myScoreThisRound > 0 ? "Correct!" : "Incorrect!"}</CardTitle>
          <CardDescription>The correct answer was:</CardDescription>
          <p className="text-xl font-bold">{results.answer}</p>
        </CardHeader>
        <CardContent>
          <p className="font-semibold">Waiting for the next question...</p>
        </CardContent>
      </Card>
    </section>
  );
}