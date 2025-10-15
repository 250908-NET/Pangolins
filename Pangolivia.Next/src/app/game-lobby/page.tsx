"use client";

import { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Users, Play, Copy, Check, UserCheck } from "lucide-react";

interface Player {
  id: string;
  name: string;
  isHost: boolean;
  joinedAt: string;
}

interface GameData {
  roomCode: string;
  name: string;
  questions: string[];    
  createdAt: string;
}

export default function GameLobbyPage() {
  return (
    <Suspense fallback={null}>
      <GameLobbyContent />
    </Suspense>
  );
}

function GameLobbyContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const roomCode = searchParams.get("room");

  const [gameData, setGameData] = useState<GameData | null>(null);
  const [currentPlayer, setCurrentPlayer] = useState<Player | null>(null);
  const [players, setPlayers] = useState<Player[]>([]);
  const [copied, setCopied] = useState(false);
  const [hostReady, setHostReady] = useState(false);

  useEffect(() => {
    if (!roomCode) {
      router.push("/join-game");
      return;
    }

    // Load game data
    const allGames = JSON.parse(localStorage.getItem("allGames") || "{}");
    const game = allGames[roomCode];

    if (!game) {
      router.push("/join-game");
      return;
    }

    setGameData(game);

    // Load current player
    const playerData = JSON.parse(
      localStorage.getItem("currentPlayer") || "null"
    );
    if (playerData && playerData.roomCode === roomCode) {
      setCurrentPlayer(playerData);
    }

    // Load players from localStorage (simulating multiplayer)
    const storedPlayers = JSON.parse(
      localStorage.getItem(`players_${roomCode}`) || "[]"
    );

    // Add current player if not already in list
    if (
      playerData &&
      !storedPlayers.find((p: Player) => p.id === playerData.id)
    ) {
      storedPlayers.push(playerData);
      localStorage.setItem(
        `players_${roomCode}`,
        JSON.stringify(storedPlayers)
      );
    }

    setPlayers(storedPlayers);
  }, [roomCode, router]);

  const handleCopyCode = async () => {
    if (roomCode) {
      await navigator.clipboard.writeText(roomCode);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  const handleToggleHostReady = () => {
    setHostReady(!hostReady);
  };

  const handleStartGame = () => {
    if (currentPlayer?.isHost && roomCode) {
      // Mark game as started
      localStorage.setItem(`game_${roomCode}_started`, "true");
      router.push(`/game-active?room=${roomCode}`);
    }
  };

  const handleLeaveGame = () => {
    if (roomCode && currentPlayer) {
      // Remove player from players list
      const updatedPlayers = players.filter((p) => p.id !== currentPlayer.id);
      localStorage.setItem(
        `players_${roomCode}`,
        JSON.stringify(updatedPlayers)
      );
      localStorage.removeItem("currentPlayer");
    }
    router.push("/join-game");
  };

  if (!gameData || !currentPlayer) {
    return null;
  }

  const isHost = currentPlayer.isHost;

  return (
    <section className="flex min-h-screen items-center justify-center bg-zinc-50 px-4 py-16 dark:bg-transparent">
      <div className="w-full max-w-2xl">
        <Card>
          <CardHeader>
            <div className="flex items-start justify-between">
              <div>
                <CardTitle className="text-3xl">{gameData.name}</CardTitle>
                <CardDescription className="mt-2">
                  Waiting for players to join...
                </CardDescription>
              </div>
              {isHost && (
                <span className="rounded-full bg-blue-100 px-3 py-1 text-xs font-semibold text-blue-600 dark:bg-blue-900/20 dark:text-blue-400">
                  HOST
                </span>
              )}
            </div>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Room Code */}
            <div className="rounded-lg border bg-zinc-50 p-4 dark:bg-zinc-900">
              <p className="text-muted-foreground mb-2 text-sm font-medium">
                Room Code
              </p>
              <div className="flex items-center justify-between">
                <p className="text-3xl font-bold tracking-widest">{roomCode}</p>
                <Button variant="outline" size="sm" onClick={handleCopyCode}>
                  {copied ? (
                    <>
                      <Check className="mr-2 h-4 w-4" />
                      Copied!
                    </>
                  ) : (
                    <>
                      <Copy className="mr-2 h-4 w-4" />
                      Copy
                    </>
                  )}
                </Button>
              </div>
              <p className="text-muted-foreground mt-2 text-xs">
                Share this code with friends to join
              </p>
            </div>

            {/* Players List */}
            <div>
              <div className="mb-3 flex items-center gap-2">
                <Users className="h-5 w-5" />
                <h3 className="text-lg font-semibold">
                  Players ({players.length})
                </h3>
              </div>
              <div className="space-y-2">
                {players.map((player) => (
                  <div
                    key={player.id}
                    className="flex items-center justify-between rounded-lg border p-3"
                  >
                    <div className="flex items-center gap-3">
                      <div className="flex h-10 w-10 items-center justify-center rounded-full bg-blue-100 dark:bg-blue-900/20">
                        <span className="font-semibold text-blue-600 dark:text-blue-400">
                          {player.name.charAt(0).toUpperCase()}
                        </span>
                      </div>
                      <div>
                        <p className="font-medium">{player.name}</p>
                        {player.id === currentPlayer.id && (
                          <p className="text-muted-foreground text-xs">You</p>
                        )}
                      </div>
                    </div>
                    {player.isHost && (
                      <span className="rounded-full bg-yellow-100 px-2 py-0.5 text-xs font-medium text-yellow-700 dark:bg-yellow-900/20 dark:text-yellow-400">
                        Host
                      </span>
                    )}
                  </div>
                ))}
              </div>
            </div>

            {/* Game Info */}
            <div className="rounded-lg border bg-zinc-50 p-4 dark:bg-zinc-900">
              <p className="text-muted-foreground text-sm">
                <span className="font-semibold">
                  {gameData.questions.length}
                </span>{" "}
                questions ready
              </p>
            </div>

            {/* Actions */}
            <div className="space-y-3">
              {isHost && (
                <Button
                  onClick={handleToggleHostReady}
                  variant={hostReady ? "default" : "outline"}
                  className="w-full"
                  size="lg"
                >
                  <UserCheck className="mr-2 h-5 w-5" />
                  {hostReady ? "Host Ready!" : "Mark Host as Ready"}
                </Button>
              )}

              <div className="flex gap-3">
                <Button
                  variant="outline"
                  onClick={handleLeaveGame}
                  className="flex-1"
                >
                  Leave Game
                </Button>
                {isHost ? (
                  <Button
                    onClick={handleStartGame}
                    disabled={!hostReady || players.length === 0}
                    className="flex-1"
                    size="lg"
                  >
                    <Play className="mr-2 h-5 w-5" />
                    Start Game
                  </Button>
                ) : (
                  <Button disabled className="flex-1" size="lg">
                    Waiting for host...
                  </Button>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </section>
  );
}
