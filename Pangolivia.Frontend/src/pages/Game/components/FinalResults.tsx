import { useMemo } from 'react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Trophy } from 'lucide-react';
import type { FinalGameRecord } from '@/contexts/SignalRContext';

interface FinalResultsProps {
  results: FinalGameRecord;
  onExitGame: () => void;
}

export function FinalResults({ results, onExitGame }: FinalResultsProps) {
  const finalLeaderboard = useMemo(() => {
    return [...results.playerScores].sort((a, b) => b.score - a.score);
  }, [results]);

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <Card className="w-full max-w-2xl">
        <CardHeader className="text-center">
          <Trophy className="mx-auto h-12 w-12 text-yellow-500" />
          <CardTitle className="text-3xl">Game Over!</CardTitle>
          <CardDescription>Here are the final results.</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {finalLeaderboard.map((player, index) => (
              <div key={player.userId} className="flex justify-between rounded-lg border p-3">
                <p className="font-bold">{index + 1}. {player.username}</p>
                <p>{player.score} Points</p>
              </div>
            ))}
          </div>
          <Button onClick={onExitGame} className="mt-6 w-full">Back to Home</Button>
        </CardContent>
      </Card>
    </section>
  );
}