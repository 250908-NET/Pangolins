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
import { Users, Play, Copy, Check, Loader2 } from "lucide-react";
import { useSignalR } from "@/hooks/useSignalR";
import { toast } from "sonner";

interface Player {
  userId: number;
  username: string;
}

interface LobbyDetails {
  quizName: string;
  creatorUsername: string;
  questionCount: number;
}

export default function GameLobbyPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const roomCode = searchParams.get("roomCode");
  // const quizIdParam = searchParams.get("quizId");
  // const quizId = quizIdParam ? parseInt(quizIdParam) : 0;

  const { connection, connectToHub, disconnect } = useSignalR();

  const [players, setPlayers] = useState<Player[]>([]);
  const [lobbyDetails, setLobbyDetails] = useState<LobbyDetails | null>(null);
  const [copied, setCopied] = useState(false);

  const currentPlayerIsHost = JSON.parse(
    localStorage.getItem("currentPlayer") || "{}"
  ).isHost;

  useEffect(() => {
    if (!roomCode ) {
      toast.error("Invalid game lobby link.");
      navigate("/join-game");
      return;
    }

    // Connect to the hub when the component mounts
    connectToHub("/gamehub");

    // Cleanup on unmount
    return () => {
      disconnect();
    };
  }, []); // Run only once

  useEffect(() => {
    if (connection && roomCode) {
      // 1. Join the game room on the server
      connection.invoke("JoinGame", roomCode).catch((err) => {
        console.error("Error invoking JoinGame:", err);
        toast.error("Failed to join the game room.");
        navigate("/join-game");
      });

      // 2. Listen for lobby details (sent on join)
      connection.on("ReceiveLobbyDetails", (details: LobbyDetails) => {
        console.log("Received lobby details:", details);
        setLobbyDetails(details);
      });
      
      // 3. Listen for full player list updates.
      connection.on("UpdatePlayerList", (playerList: Player[]) => {
        console.log("Received updated player list:", playerList);
        setPlayers(playerList);
      });

      // 4. Listen for errors from the hub
      connection.on("Error", (message: string) => {
        toast.error(message);
      });

      // Cleanup event listeners when connection or roomCode changes
      return () => {
        connection.off("ReceiveLobbyDetails");
        connection.off("UpdatePlayerList");
        connection.off("Error");
      };
    }
  }, [connection, roomCode, navigate]);

  const handleCopyCode = async () => {
    if (roomCode) {
      await navigator.clipboard.writeText(roomCode);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  const handleStartGame = () => {
    // Logic to start the game for all players will be sent from here later
    // navigate(`/game-active?quiz=${quizId}`);
  };

  const handleLeaveGame = async () => {
    localStorage.removeItem("currentPlayer");
    await disconnect(); // Explicitly disconnect from the hub
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

  return (
    <section className="flex min-h-screen items-center justify-center px-4 py-16">
      <div className="w-full max-w-2xl">
        <Card>
          <CardHeader>
            <CardTitle className="text-3xl">{lobbyDetails.quizName}</CardTitle>
            <CardDescription>
              Created by {lobbyDetails.creatorUsername} â€¢ {lobbyDetails.questionCount} questions
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
              <div className="mb-3 flex items-center gap-2">
                <Users className="h-5 w-5" />
                <h3 className="text-lg font-semibold">Players ({players.length})</h3>
              </div>
              <div className="space-y-2">
                {players.map((player) => (
                  <div key={player.userId} className="flex items-center justify-between rounded-lg border p-3">
                    <p className="font-medium">{player.username}</p>
                  </div>
                ))}
              </div>
            </div>

            <div className="flex gap-3">
              <Button variant="outline" onClick={handleLeaveGame} className="flex-1">
                Leave Game
              </Button>
              {currentPlayerIsHost && (
                <Button onClick={handleStartGame} disabled={players.length === 0} className="flex-1" size="lg">
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