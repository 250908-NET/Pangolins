import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { User, Trophy, Calendar, Loader2 } from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { useUser } from "@/hooks/useUsers";
import { usePlayerHistory, useAverageScore } from "@/hooks/usePlayerGameRecords";

export default function ProfilePage() {
  const navigate = useNavigate();
  const { user: authUser } = useAuth();
  const userId = authUser?.id || 0;

  const { data: userDetail, isLoading: userLoading } = useUser(userId);
  const { data: gameHistory, isLoading: historyLoading } = usePlayerHistory(userId);
  const { data: averageScore, isLoading: averageLoading } = useAverageScore(userId);

  const isLoading = userLoading || historyLoading || averageLoading;

  if (isLoading) {
    return (
      <section className="min-h-[calc(100vh-5rem)] px-4 py-2 flex items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </section>
    );
  }

  return (
    <section className="min-h-[calc(100vh-5rem)] px-4 py-2 flex items-center justify-center">
      <div className="w-full max-w-3xl">
        <div className="mb-8">
          <h1 className="mb-2 text-3xl font-bold">Profile</h1>
          <p className="text-muted-foreground">
            View your game history and stats
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          <Card>
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className="flex h-12 w-12 items-center justify-center rounded-full bg-blue-100 dark:bg-blue-900/20">
                  <User className="h-6 w-6 text-blue-600 dark:text-blue-400" />
                </div>
                <div>
                  <CardTitle>Player Info</CardTitle>
                  <CardDescription>Your account details</CardDescription>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-2">
              <div>
                <p className="text-sm font-medium">Username</p>
                <p className="text-lg">{userDetail?.username || 'N/A'}</p>
              </div>
              <div>
                <p className="text-sm font-medium">Quizzes Created</p>
                <p className="text-lg">{userDetail?.createdQuizzes.length || 0}</p>
              </div>
              <div>
                <p className="text-sm font-medium">Games Hosted</p>
                <p className="text-lg">{userDetail?.hostedGamesCount || 0}</p>
              </div>
              <div>
                <p className="text-sm font-medium">Games Played</p>
                <p className="text-lg">{userDetail?.gamesPlayedCount || 0}</p>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className="flex h-12 w-12 items-center justify-center rounded-full bg-yellow-100 dark:bg-yellow-900/20">
                  <Trophy className="h-6 w-6 text-yellow-600 dark:text-yellow-400" />
                </div>
                <div>
                  <CardTitle>Statistics</CardTitle>
                  <CardDescription>Your performance</CardDescription>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-2">
              <div>
                <p className="text-sm font-medium">Average Score</p>
                <p className="text-2xl font-bold">
                  {averageScore?.averageScore?.toFixed(1) || '0.0'}%
                </p>
              </div>
              <div>
                <p className="text-sm font-medium">Total Games</p>
                <p className="text-lg">{gameHistory?.length || 0}</p>
              </div>
            </CardContent>
          </Card>

          <Card className="md:col-span-2">
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className="flex h-12 w-12 items-center justify-center rounded-full bg-green-100 dark:bg-green-900/20">
                  <Calendar className="h-6 w-6 text-green-600 dark:text-green-400" />
                </div>
                <div>
                  <CardTitle>Game History</CardTitle>
                  <CardDescription>Your recent games</CardDescription>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              {gameHistory && gameHistory.length > 0 ? (
                <div className="space-y-3">
                  {gameHistory.slice(0, 5).map((record, index) => (
                    <div
                      key={record.id || index}
                      className="flex items-center justify-between border-b pb-2 last:border-b-0"
                    >
                      <div>
                        <p className="font-medium">Game #{record.gameRecordId || 'N/A'}</p>
                        <p className="text-sm text-muted-foreground">
                          Score: {record.score.toFixed(1)}%
                        </p>
                      </div>
                    </div>
                  ))}
                  {gameHistory.length > 5 && (
                    <p className="text-sm text-muted-foreground text-center pt-2">
                      Showing 5 of {gameHistory.length} games
                    </p>
                  )}
                </div>
              ) : (
                <p className="text-muted-foreground mb-4 text-sm">
                  No game history yet. Start playing to see your results here!
                </p>
              )}
              <Button onClick={() => navigate("/start-game")} className="mt-4">
                Start a New Game
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </section>
  );
}
