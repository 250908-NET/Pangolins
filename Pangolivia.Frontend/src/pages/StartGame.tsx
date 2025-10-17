import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Play, Users, Loader2 } from 'lucide-react'
import { useQuizzes } from '@/hooks/useQuizzes'


export default function StartGamePage() {
  const navigate = useNavigate()
  const { data: allQuizzes, isLoading } = useQuizzes()
  const [hostName, setHostName] = useState('')
  const [selectedQuiz, setSelectedQuiz] = useState<number | null>(null)

  const handleStartGame = (quizId: number) => {
    if (!hostName.trim()) {
      alert('Please enter your name before starting the game')
      return
    }

    // Create host player
    const hostPlayer = {
      id: crypto.randomUUID(),
      name: hostName,
      quizId: quizId,
      isHost: true,
      joinedAt: new Date().toISOString()
    }
    
    // Store host as current player
    localStorage.setItem('currentPlayer', JSON.stringify(hostPlayer))
    
    // Initialize players list with host
    localStorage.setItem(`players_${quizId}`, JSON.stringify([hostPlayer]))
    
    // Redirect to lobby
    navigate(`/game-lobby?quiz=${quizId}`)
  }

  return (
    <section className="min-h-screen px-4 py-16">
      <div className="mx-auto max-w-4xl">
        <div className="mb-8">
          <h1 className="mb-2 text-3xl font-bold">Start a Game</h1>
          <p className="text-muted-foreground">
            {"Select a game you've created and start a new session"}
          </p>
        </div>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Your Name (Host)</CardTitle>
            <CardDescription>Enter your name to host the game</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              <Label htmlFor="hostName">Host Name</Label>
              <Input
                id="hostName"
                placeholder="Enter your name..."
                value={hostName}
                onChange={(e) => setHostName(e.target.value)}
              />
            </div>
          </CardContent>
        </Card>

        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin" />
          </div>
        ) : !allQuizzes || allQuizzes.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <p className="text-muted-foreground mb-4">
                No games created yet.
              </p>
              <Button onClick={() => navigate('/create-game')}>
                Create Your First Game
              </Button>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-4">
            <h2 className="text-xl font-semibold">Available Quizzes</h2>
            {allQuizzes.map((quiz) => (
              <Card 
                key={quiz.id}
                className={`cursor-pointer transition-all ${
                  selectedQuiz === quiz.id 
                    ? 'border-blue-500 ring-2 ring-blue-200 dark:ring-blue-900' 
                    : 'hover:border-gray-400'
                }`}
                onClick={() => setSelectedQuiz(quiz.id)}
              >
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <CardTitle className="text-xl">{quiz.quizName}</CardTitle>
                      <div className="mt-2 flex flex-wrap gap-3 text-sm text-muted-foreground">
                        <div className="flex items-center gap-1">
                          <Users className="h-4 w-4" />
                          <span>{quiz.questionCount} questions</span>
                        </div>
                        <div className="flex items-center gap-1">
                          <span>By {quiz.creatorUsername}</span>
                        </div>
                      </div>
                    </div>
                    <Button
                      onClick={(e) => {
                        e.stopPropagation()
                        handleStartGame(quiz.id)
                      }}
                      disabled={!hostName.trim()}
                      size="lg"
                    >
                      <Play className="mr-2 h-5 w-5" />
                      Start Game
                    </Button>
                  </div>
                </CardHeader>
              </Card>
            ))}
          </div>
        )}

        <div className="mt-6 flex gap-3">
          <Button
            variant="outline"
            onClick={() => navigate('/create-game')}
            className="flex-1"
          >
            Create New Game
          </Button>
          <Button
            variant="outline"
            onClick={() => navigate('/edit-game')}
            className="flex-1"
          >
            Edit Games
          </Button>
        </div>
      </div>
    </section>
  )
}
