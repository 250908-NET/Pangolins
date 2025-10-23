import { useMemo } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Spinner } from "@/components/ui/spinner";
import { CheckCircle, XCircle, Trophy, Medal, Award } from "lucide-react";
import CountUp from "@/components/CountUp";
import type { RoundResults } from "@/contexts/SignalRContext";

interface RoundResultsProps {
  results: RoundResults;
  currentUserId?: number;
  isHost?: boolean;
}

export function RoundResults({
  results,
  currentUserId,
  isHost,
}: RoundResultsProps) {
  const myScoreThisRound =
    results.playerScores.find((p) => p.userId === currentUserId)?.score ?? 0;

  const leaderboard = useMemo(() => {
    // Defensively sort by totalScore, defaulting to 0 if undefined
    return [...results.playerScores].sort(
      (a, b) => (b.totalScore ?? 0) - (a.totalScore ?? 0)
    );
  }, [results]);

  // Calculate answer statistics
  const answerStats = useMemo(() => {
    const correctCount = results.playerScores.filter(
      (p) => (p.score ?? 0) > 0
    ).length;
    const incorrectCount = results.playerScores.length - correctCount;
    return {
      correctCount,
      incorrectCount,
      totalPlayers: results.playerScores.length,
    };
  }, [results.playerScores]);

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
          {isHost ? (
            <>
              <CardTitle className="text-3xl">Round Complete</CardTitle>
              <CardDescription>The correct answer was:</CardDescription>
              <p className="text-xl font-bold">{results.answer}</p>

              {results.answer1 && (
                <div className="mt-4">
                  <div className="mb-3 flex items-center justify-center gap-4 text-sm">
                    <div className="flex items-center gap-1.5">
                      <CheckCircle className="h-4 w-4 text-green-600" />
                      <span className="font-semibold text-green-600 dark:text-green-400">
                        {answerStats.correctCount} Correct
                      </span>
                    </div>
                    <div className="flex items-center gap-1.5">
                      <XCircle className="h-4 w-4 text-red-600" />
                      <span className="font-semibold text-red-600 dark:text-red-400">
                        {answerStats.incorrectCount} Incorrect
                      </span>
                    </div>
                  </div>
                  <p className="text-sm font-semibold text-muted-foreground mb-2">
                    All Answers:
                  </p>
                  <div className="space-y-1 text-sm">
                    {[
                      results.answer1,
                      results.answer2,
                      results.answer3,
                      results.answer4,
                    ].map(
                      (answer, idx) =>
                        answer && (
                          <div
                            key={idx}
                            className={`flex items-center justify-between gap-2 rounded px-3 py-1 ${
                              answer === results.answer
                                ? "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100"
                                : "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-100"
                            }`}
                          >
                            <div className="flex items-center gap-2">
                              {answer === results.answer ? (
                                <CheckCircle className="h-4 w-4" />
                              ) : (
                                <XCircle className="h-4 w-4" />
                              )}
                              <span>{answer}</span>
                            </div>
                            {answer === results.answer && (
                              <Badge
                                variant="secondary"
                                className="bg-green-200 text-green-900 dark:bg-green-800 dark:text-green-100"
                              >
                                +{answerStats.correctCount}
                              </Badge>
                            )}
                          </div>
                        )
                    )}
                  </div>
                </div>
              )}
            </>
          ) : (
            <>
              {myScoreThisRound > 0 ? (
                <CheckCircle className="mx-auto h-12 w-12 text-green-500" />
              ) : (
                <XCircle className="mx-auto h-12 w-12 text-red-500" />
              )}
              <CardTitle className="text-3xl">
                {myScoreThisRound > 0 ? "Correct!" : "Incorrect!"}
              </CardTitle>
              <CardDescription>The correct answer was:</CardDescription>
              <p className="text-xl font-bold">{results.answer}</p>
            </>
          )}
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <h3 className="text-center text-xl font-semibold">Leaderboard</h3>
            <div className="space-y-3">
              {leaderboard.map((player, index) => {
                const roundScore = player.score ?? 0;
                const totalScore = player.totalScore ?? 0;
                const isTopThree = index < 3;
                const isCurrentUser = player.userId === currentUserId;

                return (
                  <div
                    key={player.userId}
                    className={`flex items-center justify-between rounded-lg border p-3 transition-all ${
                      isCurrentUser
                        ? "border-primary bg-primary/5 ring-2 ring-primary/20"
                        : isTopThree
                        ? "border-muted-foreground/20 bg-muted/30"
                        : ""
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
                        <AvatarFallback
                          className={`text-xs font-semibold ${
                            index === 0
                              ? "bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-100"
                              : index === 1
                              ? "bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-100"
                              : index === 2
                              ? "bg-amber-100 text-amber-700 dark:bg-amber-900 dark:text-amber-100"
                              : ""
                          }`}
                        >
                          {player.username.slice(0, 2).toUpperCase()}
                        </AvatarFallback>
                      </Avatar>
                      <div className="flex flex-col">
                        <p className="font-medium leading-none">
                          {player.username}
                        </p>
                        {isTopThree && (
                          <div className="mt-1">{getMedalBadge(index)}</div>
                        )}
                      </div>
                    </div>
                    <div className="text-right">
                      {player.userId === currentUserId || isHost ? (
                        <>
                          <p className="text-lg font-bold">
                            <CountUp
                              from={totalScore - roundScore}
                              to={totalScore}
                              duration={0.8}
                              separator=","
                            />
                          </p>
                          {roundScore > 0 && (
                            <p className="text-sm font-semibold text-green-600 dark:text-green-400">
                              +
                              <CountUp
                                from={0}
                                to={roundScore}
                                duration={0.8}
                                separator=","
                              />
                            </p>
                          )}
                        </>
                      ) : (
                        <>
                          <p className="text-lg font-bold">
                            {totalScore.toLocaleString()}
                          </p>
                          {roundScore > 0 && (
                            <p className="text-sm font-semibold text-green-600 dark:text-green-400">
                              +{roundScore.toLocaleString()}
                            </p>
                          )}
                        </>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
          <div className="mt-6 flex items-center justify-center gap-3">
            <Spinner className="h-5 w-5 text-primary" />
            <p className="text-center font-semibold text-muted-foreground">
              Waiting for the next question...
            </p>
          </div>
        </CardContent>
      </Card>
    </section>
  );
}
