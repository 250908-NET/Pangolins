import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Users, Play, Copy, Check, Loader2, Crown } from "lucide-react";
import { useSignalR } from "@/hooks/useSignalR";
import { toast } from "sonner";

export default function GameLobbyPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const roomCode = searchParams.get("roomCode");

  // --- Consume centralized state and actions from the hook ---
  const {
    connection,
    disconnect,
    joinLobby,
    startLobbyGame,
    lobbyDetails,
    players,
    gameStarted,
  } = useSignalR();

  const [copied, setCopied] = useState(false);
  const currentPlayerIsHost = JSON.parse(
    localStorage.getItem("currentPlayer") || "{}"
  ).isHost;

  // --- Effect to join the lobby on component mount ---
  useEffect(() => {
    if (connection && roomCode) {
      joinLobby(roomCode).catch((err) => {
        console.error("Error invoking JoinGame:", err);
        toast.error("Failed to join the game room.");
        navigate("/join-game");
      });
    }
  }, [connection, roomCode, joinLobby, navigate]);

  // --- Effect to navigate away when the game starts ---
  useEffect(() => {
    if (gameStarted && roomCode) {
      toast.info("The game is starting!");
      navigate(`/game-active?roomCode=${roomCode}`);
    }
  }, [gameStarted, roomCode, navigate]);

  const handleCopyCode = async () => {
    if (roomCode) {
      await navigator.clipboard.writeText(roomCode);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  const handleStartGame = () => {
    if (roomCode && currentPlayerIsHost) {
      // Call the action from the context
      startLobbyGame(roomCode).catch((err) => {
        console.error("Error starting game:", err);
        toast.error(`Failed to start game: ${err.message}`);
      });
    }
  };

  const handleLeaveGame = async () => {
    localStorage.removeItem("currentPlayer");
    // The disconnect function from the context will handle stopping the connection
    // and resetting the game state.
    await disconnect();
    navigate("/join-game");
  };

  if (!connection || !lobbyDetails) {
    return (
      <section className="flex min-h-screen items-center justify-center px-4 py-16">
        <div className="flex items-center gap-2">
          <Loader2 className="h-8 w-8 animate-spin" />
          <span className="text-lg">
            {!connection ? "Connecting to lobby..." : "Loading quiz details..."}
          </span>
        </div>
      </section>
    );
  }

  // Derive host and players directly from the centralized state
  const host = players.find((p) => p.isHost);
  const otherPlayers = players.filter((p) => !p.isHost);
  const totalPlayers = players.length;

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <div className="w-full max-w-2xl">
        <Card>
          <CardHeader>
            <CardTitle className="text-3xl">{lobbyDetails.quizName}</CardTitle>
            <CardDescription>
              Created by {lobbyDetails.creatorUsername} | {lobbyDetails.questionCount} questions
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="rounded-lg border bg-zinc-50 p-4 dark:bg-zinc-900">
              <p className="text-muted-foreground mb-2 text-sm font-medium">
                Room Code
              </p>
              <div className="flex items-center justify-between">
                <p className="text-3xl font-bold tracking-widest">{roomCode}</p>
                <Button variant="outline" size="sm" onClick={handleCopyCode}>
                  {copied ? <Check className="mr-2 h-4 w-4" /> : <Copy className="mr-2 h-4 w-4" />}
                  {copied ? 'Copied!' : 'Copy'}
                </Button>
              </div>
            </div>

            <div>
              <div className="mb-2 flex items-center gap-2">
                <Crown className="h-5 w-5 text-yellow-500" />
                <h3 className="text-lg font-semibold">Host</h3>
              </div>
              {host ? (
                <div className="flex items-center justify-between rounded-lg border-2 border-yellow-500 bg-yellow-50 p-3 dark:bg-yellow-900/20">
                  <p className="font-bold">{lobbyDetails.hostUsername}</p>
                </div>
              ) : (
                <p className="text-sm text-muted-foreground">Waiting for host...</p>
              )}

              <div className="mt-4">
                <div className="mb-2 flex items-center gap-2">
                  <Users className="h-5 w-5" />
                  <h3 className="text-lg font-semibold">Players ({totalPlayers})</h3>
                </div>
                <div className="space-y-2">
                  {otherPlayers.length > 0 ? (
                    otherPlayers.map((player) => (
                      <div
                        key={player.userId}
                        className="flex items-center justify-between rounded-lg border p-3"
                      >
                        <p className="font-medium">{player.username}</p>
                      </div>
                    ))
                  ) : (
                    <p className="text-sm text-muted-foreground">
                      No other players have joined yet.
                    </p>
                  )}
                </div>
              </div>
            </div>

            <div className="flex gap-3">
              <Button variant="outline" onClick={handleLeaveGame} className="flex-1">
                Leave Game
              </Button>
              {currentPlayerIsHost && (
                <Button onClick={handleStartGame} className="flex-1" size="lg">
                  <Play className="mr-2 h-5 w-5" />
                  Start Game
                </Button>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </section>
  );
}