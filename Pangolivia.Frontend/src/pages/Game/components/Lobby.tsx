import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Users, Play, Copy, Check, Crown } from 'lucide-react';
import type { LobbyDetails, Player } from '@/contexts/SignalRContext';

interface LobbyProps {
  roomCode: string;
  lobbyDetails: LobbyDetails;
  players: Player[];
  isHost: boolean;
  onStartGame: () => void;
  onLeaveGame: () => void;
}

export function Lobby({ roomCode, lobbyDetails, players, isHost, onStartGame, onLeaveGame }: LobbyProps) {
  const [copied, setCopied] = useState(false);

  const handleCopyCode = async () => {
    await navigator.clipboard.writeText(roomCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

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
              <p className="text-muted-foreground mb-2 text-sm font-medium">Room Code</p>
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
                      <div key={player.userId} className="flex items-center justify-between rounded-lg border p-3">
                        <p className="font-medium">{player.username}</p>
                      </div>
                    ))
                  ) : (
                    <p className="text-sm text-muted-foreground">No other players have joined yet.</p>
                  )}
                </div>
              </div>
            </div>

            <div className="flex gap-3">
              <Button variant="outline" onClick={onLeaveGame} className="flex-1">
                Leave Game
              </Button>
              {isHost && (
                <Button onClick={onStartGame} className="flex-1" size="lg">
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