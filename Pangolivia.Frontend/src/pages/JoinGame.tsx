import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { LogIn, AlertCircle, Loader2 } from 'lucide-react'
import { useMutation } from '@tanstack/react-query'
import { quizService } from '@/services/quizService'
import { AxiosError } from 'axios'

export default function JoinGamePage() {
  const navigate = useNavigate()
  const [quizId, setQuizId] = useState('')
  const [playerName, setPlayerName] = useState('')
  const [error, setError] = useState('')

  const handleQuizIdChange = (value: string) => {
    // Only allow numbers
    const numericValue = value.replace(/\D/g, '')
    setQuizId(numericValue)
    setError('')
  }

  // Mutation to validate quiz and join game
  const joinGameMutation = useMutation({
    mutationFn: async ({ quizId, playerName }: { quizId: number; playerName: string }) => {
      // Verify quiz exists
      const quiz = await quizService.getQuizById(quizId)
      return { quiz, quizId, playerName }
    },
    onSuccess: ({ quizId, playerName }) => {
      // Create player ID and store player info
      const playerId = crypto.randomUUID()
      const playerData = {
        id: playerId,
        name: playerName,
        quizId: quizId,
        isHost: false,
        joinedAt: new Date().toISOString()
      }

      // Store current player data
      localStorage.setItem('currentPlayer', JSON.stringify(playerData))

      // Add player to the quiz's player list
      const existingPlayers = JSON.parse(
        localStorage.getItem(`players_${quizId}`) || '[]'
      )
      const updatedPlayers = [...existingPlayers, playerData]
      localStorage.setItem(`players_${quizId}`, JSON.stringify(updatedPlayers))

      // Redirect to game lobby
      navigate(`/game-lobby?quiz=${quizId}`)
    },
    onError: (err: unknown) => {
      console.error('Error joining game:', err)
      const error = err as AxiosError
      if (error.response?.status === 404) {
        setError('Quiz not found. Please check the Quiz ID.')
      } else if (error.message) {
        setError(error.message)
      } else {
        setError('Failed to join game. Please try again.')
      }
    },
  })

  const handleJoinGame = () => {
    if (!playerName.trim()) {
      setError('Please enter your name')
      return
    }

    if (!quizId) {
      setError('Please enter a quiz ID')
      return
    }

    setError('')
    joinGameMutation.mutate({ quizId: parseInt(quizId), playerName })
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && playerName.trim() && quizId && !joinGameMutation.isPending) {
      handleJoinGame()
    }
  }

  return (
    <section className="flex min-h-[calc(100vh-5rem)] items-center justify-center px-4">
      <div className="w-full max-w-md">
        <Card>
          <CardHeader className="text-center">
            <CardTitle className="text-3xl">Join Game</CardTitle>
            <CardDescription>
              Enter your name and the Quiz ID to join
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
              <Label htmlFor="quizId" className="text-base">
                Quiz ID
              </Label>
              <Input
                id="quizId"
                type="text"
                inputMode="numeric"
                placeholder="Enter Quiz ID..."
                value={quizId}
                onChange={(e) => handleQuizIdChange(e.target.value)}
                onKeyPress={handleKeyPress}
              />
              <p className="text-muted-foreground text-xs">
                Ask the host for the Quiz ID
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
              disabled={!playerName.trim() || !quizId || joinGameMutation.isPending}
              className="w-full"
              size="lg"
            >
              {joinGameMutation.isPending ? (
                <>
                  <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                  Joining...
                </>
              ) : (
                <>
                  <LogIn className="mr-2 h-5 w-5" />
                  Join Game
                </>
              )}
            </Button>

            <div className="text-center">
              <p className="text-muted-foreground text-sm">
                {"Don't have a room code?"}{' '}
                <Link
                  to="/create-game"
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
