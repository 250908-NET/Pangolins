import { useMemo } from 'react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Trophy, Medal, Award } from 'lucide-react';
import type { FinalGameRecord } from '@/contexts/SignalRContext';

interface FinalResultsProps {
  results: FinalGameRecord;
  onExitGame: () => void;
}

export function FinalResults({ results, onExitGame }: FinalResultsProps) {
  const finalLeaderboard = useMemo(() => {
    return [...results.playerScores].sort((a, b) => b.score - a.score);
  }, [results]);

  const getMedalIcon = (position: number) => {
    switch (position) {
      case 0:
        return <Trophy className="h-5 w-5 text-yellow-500" />;
      case 1:
        return <Medal className="h-5 w-5 text-gray-400" />;
      case 2:
        return <Award className="h-5 w-5 text-amber-600" />;
      default:
        return null;
    }
  };

  const getMedalBadge = (position: number) => {
    switch (position) {
      case 0:
        return (
          <Badge className="bg-yellow-500 text-yellow-950 hover:bg-yellow-600">
            1st
          </Badge>
        );
      case 1:
        return (
          <Badge className="bg-gray-400 text-gray-950 hover:bg-gray-500">
            2nd
          </Badge>
        );
      case 2:
        return (
          <Badge className="bg-amber-600 text-amber-950 hover:bg-amber-700">
            3rd
          </Badge>
        );
      default:
        return null;
    }
  };

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <Card className="w-full max-w-2xl">
        <CardHeader className="text-center">
          <Trophy className="mx-auto h-12 w-12 text-yellow-500" />
          <CardTitle className="text-3xl">Game Over!</CardTitle>
          <CardDescription>Here are the final results.</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <h3 className="text-center text-xl font-semibold">Final Leaderboard</h3>
            <div className="space-y-3">
              {finalLeaderboard.map((player, index) => {
                const isTopThree = index < 3;
                const score = player.score ?? 0;

                return (
                  <div
                    key={player.userId}
                    className={`flex items-center justify-between rounded-lg border p-3 transition-all ${
                      isTopThree
                        ? 'border-muted-foreground/20 bg-muted/30'
                        : ''
                    }`}
                  >
                    <div className="flex items-center gap-3">
                      <div className="flex w-8 items-center justify-center">
                        {isTopThree ? (
                          getMedalIcon(index)
                        ) : (
                          <span className="text-sm font-bold text-muted-foreground">
                            {index + 1}
                          </span>
                        )}
                      </div>
                      <Avatar className="h-9 w-9">
                        <AvatarImage src="/logo.png" alt={player.username} />
                        <AvatarFallback className={`text-xs font-semibold ${
                          index === 0
                            ? 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-100'
                            : index === 1
                            ? 'bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-100'
                            : index === 2
                            ? 'bg-amber-100 text-amber-700 dark:bg-amber-900 dark:text-amber-100'
                            : ''
                        }`}>
                          {player.username.slice(0, 2).toUpperCase()}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex flex-col">
                        <p className="font-medium leading-none">{player.username}</p>
                        {isTopThree && (
                          <div className="mt-1">{getMedalBadge(index)}</div>
                        )}
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="text-lg font-bold">{score.toLocaleString()}</p>
                      <p className="text-xs text-muted-foreground">points</p>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
          <Button onClick={onExitGame} className="mt-6 w-full">
            Back to Home
          </Button>
        </CardContent>
      </Card>
    </section>
  );
}