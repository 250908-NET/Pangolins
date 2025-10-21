import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  // CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { Play, Users, Loader2 } from 'lucide-react'
import { useQuizzes } from '@/hooks/useQuizzes'
import { useAuth } from '@/contexts/AuthContext'
import { toast } from 'sonner'
import { useCreateGame } from '@/hooks/useGames'

export default function StartGamePage() {
  const navigate = useNavigate()
  const { user } = useAuth()
  const { data: allQuizzes, isLoading } = useQuizzes()
  const [selectedQuiz, setSelectedQuiz] = useState<number | null>(null)

  const createGameMutation = useCreateGame()

  const handleStartGame = (quizId: number) => {
    if (!user) {
      toast.error('You must be logged in to start a game.')
      return
    }
    createGameMutation.mutate(quizId)
  }

  return (
    <section className="min-h-[calc(100vh-5rem)] px-4 py-2 flex justify-center items-center">
      <div className="w-full max-w-3xl">
        <div className="mb-8">
          <h1 className="mb-2 text-3xl font-bold">Start a Game</h1>
          <p className="text-muted-foreground">
            Select a game you've created and start a new session
          </p>
        </div>

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
                className={`transition-all ${
                  selectedQuiz === quiz.id
                    ? 'border-blue-500 ring-2 ring-blue-200 dark:ring-blue-900'
                    : 'hover:border-gray-400'
                }`}
              >
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div
                      className="flex-1"
                      onClick={() => setSelectedQuiz(quiz.id)}
                    >
                      <CardTitle className="text-xl">
                        {quiz.quizName}
                      </CardTitle>
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
                      disabled={createGameMutation.isPending}
                      size="lg"
                    >
                      {createGameMutation.isPending &&
                      createGameMutation.variables === quiz.id ? (
                        <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                      ) : (
                        <Play className="mr-2 h-5 w-5" />
                      )}
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