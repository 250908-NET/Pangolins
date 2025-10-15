'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { LogIn, AlertCircle } from 'lucide-react'
import Link from 'next/link'

export default function JoinGamePage() {
  const router = useRouter()
  const [roomCode, setRoomCode] = useState('')
  const [playerName, setPlayerName] = useState('')
  const [error, setError] = useState('')

  const handleRoomCodeChange = (value: string) => {
    // Only allow numbers and limit to 6 digits
    const numericValue = value.replace(/\D/g, '').slice(0, 6)
    setRoomCode(numericValue)
    setError('')
  }

  const handleJoinGame = () => {
    if (!playerName.trim()) {
      setError('Please enter your name')
      return
    }

    if (roomCode.length !== 6) {
      setError('Room code must be 6 digits')
      return
    }

    // Retrieve game from localStorage
    const allGames = JSON.parse(localStorage.getItem('allGames') || '{}')
    const game = allGames[roomCode]

    if (!game) {
      setError('Game not found. Please check the room code.')
      return
    }

    // Create player ID and store player info
    const playerId = crypto.randomUUID()
    const playerData = {
      id: playerId,
      name: playerName,
      roomCode: roomCode,
      isHost: false,
      joinedAt: new Date().toISOString()
    }

    // Store current player data
    localStorage.setItem('currentPlayer', JSON.stringify(playerData))

    // Redirect to game lobby
    router.push(`/game-lobby?room=${roomCode}`)
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && playerName.trim() && roomCode.length === 6) {
      handleJoinGame()
    }
  }

  return (
    <section className="flex min-h-screen items-center justify-center bg-zinc-50 px-4 py-16 dark:bg-transparent">
      <div className="w-full max-w-md">
        <Card>
          <CardHeader className="text-center">
            <CardTitle className="text-3xl">Join Game</CardTitle>
            <CardDescription>
              Enter your name and the 6-digit room code
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="playerName" className="text-base">
                Your Name
              </Label>
              <Input
                id="playerName"
                type="text"
                placeholder="Enter your name..."
                value={playerName}
                onChange={(e) => setPlayerName(e.target.value)}
                onKeyPress={handleKeyPress}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="roomCode" className="text-base">
                Room Code
              </Label>
              <Input
                id="roomCode"
                type="text"
                inputMode="numeric"
                placeholder="000000"
                value={roomCode}
                onChange={(e) => handleRoomCodeChange(e.target.value)}
                onKeyPress={handleKeyPress}
                className="text-center text-2xl font-bold tracking-widest"
                maxLength={6}
              />
              <p className="text-muted-foreground text-xs">
                {roomCode.length}/6 digits
              </p>
            </div>

            {error && (
              <div className="flex items-start gap-2 rounded-lg bg-red-50 p-3 dark:bg-red-900/20">
                <AlertCircle className="mt-0.5 h-4 w-4 text-red-600 dark:text-red-400" />
                <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
              </div>
            )}

            <Button
              onClick={handleJoinGame}
              disabled={!playerName.trim() || roomCode.length !== 6}
              className="w-full"
              size="lg"
            >
              <LogIn className="mr-2 h-5 w-5" />
              Join Game
            </Button>

            <div className="text-center">
              <p className="text-muted-foreground text-sm">
                {"Don't have a room code?"}{' '}
                <Link
                  href="/create-game"
                  className="font-medium text-blue-600 hover:underline dark:text-blue-400"
                >
                  Create a game
                </Link>
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </section>
  )
}
