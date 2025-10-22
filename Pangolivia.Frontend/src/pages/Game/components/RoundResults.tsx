import { useMemo } from 'react';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import { CheckCircle, XCircle } from 'lucide-react';
import type { RoundResults } from '@/contexts/SignalRContext';

interface RoundResultsProps {
  results: RoundResults;
  currentUserId?: number;
}

export function RoundResults({ results, currentUserId }: RoundResultsProps) {
  const myScoreThisRound = results.playerScores.find((p) => p.userId === currentUserId)?.score ?? 0;

  const leaderboard = useMemo(() => {
    // Defensively sort by totalScore, defaulting to 0 if undefined
    return [...results.playerScores].sort((a, b) => (b.totalScore ?? 0) - (a.totalScore ?? 0));
  }, [results]);

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <Card className="w-full max-w-2xl">
        <CardHeader className="text-center">
          {myScoreThisRound > 0 ? (
            <CheckCircle className="mx-auto h-12 w-12 text-green-500" />
          ) : (
            <XCircle className="mx-auto h-12 w-12 text-red-500" />
          )}
          <CardTitle className="text-3xl">
            {myScoreThisRound > 0 ? 'Correct!' : 'Incorrect!'}
          </CardTitle>
          <CardDescription>The correct answer was:</CardDescription>
          <p className="text-xl font-bold">{results.answer}</p>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <h3 className="text-center text-xl font-semibold">Leaderboard</h3>
            <div className="space-y-2">
              {leaderboard.map((player, index) => {
                const roundScore = player.score ?? 0;
                const totalScore = player.totalScore ?? 0;

                return (
                  <div
                    key={player.userId}
                    className={`flex items-center justify-between rounded-lg border p-3 ${
                      player.userId === currentUserId
                        ? 'border-primary ring-2 ring-primary/20'
                        : ''
                    }`}
                  >
                    <div className="flex items-center gap-3">
                      <span className="w-6 text-center font-bold text-muted-foreground">
                        {index + 1}
                      </span>
                      <p className="font-medium">{player.username}</p>
                    </div>
                    <div className="text-right">
                      <p className="font-bold">{totalScore.toLocaleString()}</p>
                      {roundScore > 0 && (
                        <p className="text-sm text-green-600">
                          +{roundScore.toLocaleString()}
                        </p>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
          <p className="mt-6 text-center font-semibold text-muted-foreground">
            Waiting for the next question...
          </p>
        </CardContent>
      </Card>
    </section>
  );
}