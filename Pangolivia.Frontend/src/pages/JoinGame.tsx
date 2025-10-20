import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { LogIn, AlertCircle, Loader2 } from 'lucide-react'
import { useMutation } from '@tanstack/react-query'
import { quizService } from '@/services/quizService'

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
    onError: (err: any) => {
      console.error('Error joining game:', err)
      if (err.response?.status === 404) {
        setError('Quiz not found. Please check the Quiz ID.')
      } else if (err.message) {
        setError(err.message)
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
        <Card className="bg-zinc-800 border border-zinc-700">
          <CardHeader className="text-center">
            <CardTitle className="text-3xl text-white">Join Game</CardTitle>
            <CardDescription className="text-zinc-200">
              Enter your name and the Quiz ID to join
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="playerName" className="text-base text-white">
                Your Name
              </Label>
              <Input
                id="playerName"
                type="text"
                placeholder="Enter your name..."
                value={playerName}
                onChange={(e) => setPlayerName(e.target.value)}
                onKeyPress={handleKeyPress}
                className="bg-zinc-900 text-white placeholder-zinc-300 border-zinc-700"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="quizId" className="text-base text-white">
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
                className="bg-zinc-900 text-white placeholder-zinc-300 border-zinc-700"
              />
              <p className="text-xs text-zinc-200">
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
              className="w-full bg-yellow-400 text-black font-semibold hover:bg-yellow-500 focus:ring-2 focus:ring-yellow-400"
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
              <p className="text-sm text-zinc-200">
                {"Don't have a room code?"}{' '}
                <Link
                  to="/create-game"
                  className="font-semibold text-yellow-400 hover:underline"
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
