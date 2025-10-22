import { useState, useMemo } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useNavigate } from "react-router-dom";
import { User, Trophy, Calendar, Loader2, Star, Info } from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { useUser } from "@/hooks/useUsers";
import { usePlayerHistory, useAverageScore, useLeaderboard } from "@/hooks/usePlayerGameRecords";
import { useGameRecord } from "@/hooks/useGameRecords";

type SortOption = 'newest' | 'oldest' | 'alphabetical';

export default function ProfilePage() {
  const navigate = useNavigate();
  const { user: authUser } = useAuth();
  const userId = authUser?.id || 0;
  const [selectedGameRecordId, setSelectedGameRecordId] = useState<number | null>(null);
  const [sortBy, setSortBy] = useState<SortOption>('newest');

  const { data: userDetail, isLoading: userLoading } = useUser(userId);
  const { data: gameHistory, isLoading: historyLoading } = usePlayerHistory(userId);
  const { data: averageScore, isLoading: averageLoading } = useAverageScore(userId);
  const { data: selectedGame, isLoading: gameLoading } = useGameRecord(selectedGameRecordId || 0);
  const { data: leaderboard, isLoading: leaderboardLoading } = useLeaderboard(selectedGameRecordId || 0);

  const isLoading = userLoading || historyLoading || averageLoading;

  // Sort game history based on selected option
  const sortedGameHistory = useMemo(() => {
    if (!gameHistory) return [];
    
    const historyCopy = [...gameHistory];
    
    switch (sortBy) {
      case 'newest':
        return historyCopy.sort((a, b) => (b.id || 0) - (a.id || 0));
      case 'oldest':
        return historyCopy.sort((a, b) => (a.id || 0) - (b.id || 0));
      case 'alphabetical':
        // We'll need to fetch game names for sorting, so we'll sort by gameRecordId for now
        // The actual alphabetical sorting will happen in the rendered component
        return historyCopy;
      default:
        return historyCopy;
    }
  }, [gameHistory, sortBy]);

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
                <div className="flex items-center gap-2">
                  <p className="text-2xl font-bold">
                    {averageScore?.averageScore?.toFixed(0) || '0'}
                  </p>
                  <Star className="h-5 w-5 text-yellow-500" />
                </div>
              </div>
              <div>
                <p className="text-sm font-medium">Total Games</p>
                <p className="text-lg">{gameHistory?.length || 0}</p>
              </div>
            </CardContent>
          </Card>

          <Card className="md:col-span-2">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-full bg-green-100 dark:bg-green-900/20">
                    <Calendar className="h-6 w-6 text-green-600 dark:text-green-400" />
                  </div>
                  <div>
                    <CardTitle>Game History</CardTitle>
                    <CardDescription>Your recent games</CardDescription>
                  </div>
                </div>
                {gameHistory && gameHistory.length > 0 && (
                  <Select value={sortBy} onValueChange={(value) => setSortBy(value as SortOption)}>
                    <SelectTrigger className="w-[180px]">
                      <SelectValue placeholder="Sort by" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="newest">Newest First</SelectItem>
                      <SelectItem value="oldest">Oldest First</SelectItem>
                      <SelectItem value="alphabetical">Alphabetical</SelectItem>
                    </SelectContent>
                  </Select>
                )}
              </div>
            </CardHeader>
            <CardContent>
              {sortedGameHistory && sortedGameHistory.length > 0 ? (
                <div className="space-y-3">
                  <SortedGameHistoryList
                    gameHistory={sortedGameHistory.slice(0, 5)}
                    sortBy={sortBy}
                    onViewDetails={(gameRecordId) => setSelectedGameRecordId(gameRecordId)}
                  />
                  {sortedGameHistory.length > 5 && (
                    <p className="text-sm text-muted-foreground text-center pt-2">
                      Showing 5 of {sortedGameHistory.length} games
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

      {/* Game Details Dialog */}
      <Dialog open={!!selectedGameRecordId} onOpenChange={(open) => !open && setSelectedGameRecordId(null)}>
        <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Game Details</DialogTitle>
            <DialogDescription>
              {selectedGame ? `${selectedGame.quizName} - Hosted by ${selectedGame.hostUsername}` : 'Loading...'}
            </DialogDescription>
          </DialogHeader>
          
          {gameLoading || leaderboardLoading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
          ) : selectedGame && leaderboard ? (
            <div className="space-y-6">
              {/* Game Info */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">Quiz Name</p>
                  <p className="text-lg font-semibold">{selectedGame.quizName}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">Host</p>
                  <p className="text-lg">{selectedGame.hostUsername}</p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">Completed</p>
                  <p className="text-lg">
                    {new Date(selectedGame.dateTimeCompleted).toLocaleDateString()}
                  </p>
                </div>
                <div>
                  <p className="text-sm font-medium text-muted-foreground">Players</p>
                  <p className="text-lg">{leaderboard.length}</p>
                </div>
              </div>

              {/* Leaderboard */}
              <div>
                <h3 className="text-lg font-semibold mb-3 flex items-center gap-2">
                  <Trophy className="h-5 w-5 text-yellow-500" />
                  Leaderboard
                </h3>
                <div className="space-y-2">
                  {leaderboard.map((entry) => (
                    <div
                      key={entry.rank}
                      className={`flex items-center justify-between p-3 rounded-lg border ${
                        entry.username === authUser?.name ? 'bg-primary/5 border-primary' : 'bg-muted/50'
                      }`}
                    >
                      <div className="flex items-center gap-3">
                        <div className="flex items-center justify-center w-8 h-8 rounded-full bg-muted font-bold">
                          {entry.rank}
                        </div>
                        <div>
                          <p className="font-medium">
                            {entry.username}
                            {entry.username === authUser?.name && (
                              <span className="ml-2 text-xs text-primary">(You)</span>
                            )}
                          </p>
                        </div>
                      </div>
                      <Badge variant="secondary" className="flex items-center gap-1">
                        <Star className="h-3 w-3" />
                        {entry.score.toFixed(0)} pts
                      </Badge>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          ) : (
            <p className="text-center text-muted-foreground py-8">No game data available</p>
          )}
        </DialogContent>
      </Dialog>
    </section>
  );
}

// Sorted Game History List Component
function SortedGameHistoryList({
  gameHistory,
  sortBy,
  onViewDetails,
}: {
  gameHistory: Array<{ id?: number; gameRecordId?: number; score: number }>;
  sortBy: SortOption;
  onViewDetails: (gameRecordId: number | null) => void;
}) {
  // Fetch all game data for sorting
  const gamesData = gameHistory.map((record) => {
    // eslint-disable-next-line react-hooks/rules-of-hooks
    const { data: game } = useGameRecord(record.gameRecordId || 0);
    return { record, game };
  });

  // Sort alphabetically if needed
  const sortedGames = useMemo(() => {
    if (sortBy === 'alphabetical') {
      return [...gamesData].sort((a, b) => {
        const nameA = a.game?.quizName || '';
        const nameB = b.game?.quizName || '';
        return nameA.localeCompare(nameB);
      });
    }
    return gamesData;
  }, [gamesData, sortBy]);

  return (
    <>
      {sortedGames.map(({ record, game }, index) => (
        <div
          key={record.id || index}
          className="flex items-center justify-between border-b pb-3 last:border-b-0"
        >
          <div className="flex-1">
            <p className="font-medium">{game?.quizName || 'Loading...'}</p>
            <div className="flex items-center gap-2 mt-1">
              <Badge variant="outline" className="flex items-center gap-1">
                <Star className="h-3 w-3" />
                {record.score.toFixed(0)} pts
              </Badge>
            </div>
          </div>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => onViewDetails(record.gameRecordId || null)}
            disabled={!record.gameRecordId}
          >
            <Info className="h-4 w-4 mr-1" />
            Details
          </Button>
        </div>
      ))}
    </>
  );
}
