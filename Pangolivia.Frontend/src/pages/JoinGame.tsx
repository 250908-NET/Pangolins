import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { LogIn, AlertCircle, Loader2 } from 'lucide-react'
import { useJoinGame } from '@/hooks/useGames'

export default function JoinGamePage() {
  const [roomCode, setRoomCode] = useState('')
  const [error, setError] = useState('')

  const joinGameMutation = useJoinGame()

  const handleJoinGame = () => {
    if (!roomCode.trim()) {
      setError('Please enter a room code')
      return
    }
    setError('')
    joinGameMutation.mutate(roomCode.trim())
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && roomCode.trim() && !joinGameMutation.isPending) {
      handleJoinGame()
    }
  }

  return (
    <section className="flex min-h-[calc(100vh-5rem)] items-center justify-center px-4">
      <div className="w-full max-w-md">
        <Card>
          <CardHeader className="text-center">
            <CardTitle className="text-3xl">Join Game</CardTitle>
            <CardDescription>Enter the Room Code to join</CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="roomCode" className="text-base">
                Room Code
              </Label>
              <Input
                id="roomCode"
                type="text"
                placeholder="Enter 6-letter room code..."
                value={roomCode}
                onChange={(e) => setRoomCode(e.target.value.toUpperCase())}
                onKeyPress={handleKeyPress}
                maxLength={6}
              />
              <p className="text-muted-foreground text-xs">
                Ask the host for the Room Code
              </p>
            </div>

            {error && (
              <div
                id="join-game-error"
                role="alert"
                aria-live="assertive"
                className="flex items-start gap-2 rounded-lg bg-red-50 p-3 dark:bg-red-900/20">
                <AlertCircle className="mt-0.5 h-4 w-4 text-red-600 dark:text-red-400" aria-hidden="true" />
                <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
              </div>
            )}

            <Button
              onClick={handleJoinGame}
              disabled={!roomCode.trim() || joinGameMutation.isPending}
              className="w-full"
              size="lg"
            >
              {joinGameMutation.isPending ? (
                <>
                  <Loader2 className="mr-2 h-5 w-5 animate-spin" aria-hidden="true" />
                  Joining...
                </>
              ) : (
                <>
                  <LogIn className="mr-2 h-5 w-5" aria-hidden="true" />
                  Join Game
                </>
              )}
            </Button>

            <div className="text-center">
              <p className="text-muted-foreground text-sm">
                Want to host?{' '}
                <Link
                  to="/start-game"
                  className="font-medium text-primary hover:underline"
                >
                  Start a game
                </Link>
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </section>
  )
}