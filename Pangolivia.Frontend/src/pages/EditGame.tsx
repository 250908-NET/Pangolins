import { useState, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Trash2, Edit, ArrowLeft } from 'lucide-react'

interface Answer {
  id: string
  text: string
  isCorrect: boolean
}

interface Question {
  id: string
  text: string
  answers: Answer[]
}

interface GameData {
  roomCode: string
  name: string
  questions: Question[]
  createdAt: string
}

export default function EditGamePage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const editRoomCode = searchParams.get('room')

  const [allGames, setAllGames] = useState<Record<string, GameData>>({})
  const [editingGame, setEditingGame] = useState<GameData | null>(null)
  const [gameName, setGameName] = useState('')
  const [questions, setQuestions] = useState<Question[]>([])

  useEffect(() => {
    loadGames()
  }, [])

  useEffect(() => {
    if (editRoomCode && allGames[editRoomCode]) {
      const game = allGames[editRoomCode]
      setEditingGame(game)
      setGameName(game.name)
      setQuestions(game.questions)
    }
  }, [editRoomCode, allGames])

  const loadGames = () => {
    const games = JSON.parse(localStorage.getItem('allGames') || '{}')
    setAllGames(games)
  }

  const handleSelectGame = (roomCode: string) => {
    navigate(`/edit-game?room=${roomCode}`)
  }

  const handleBackToList = () => {
    setEditingGame(null)
    setGameName('')
    setQuestions([])
    navigate('/edit-game')
  }

  const addQuestion = () => {
    const newQuestion: Question = {
      id: crypto.randomUUID(),
      text: '',
      answers: []
    }
    setQuestions([...questions, newQuestion])
  }

  const deleteQuestion = (questionId: string) => {
    setQuestions(questions.filter(q => q.id !== questionId))
  }

  const updateQuestionText = (questionId: string, text: string) => {
    setQuestions(questions.map(q => 
      q.id === questionId ? { ...q, text } : q
    ))
  }

  const addAnswer = (questionId: string) => {
    const newAnswer: Answer = {
      id: crypto.randomUUID(),
      text: '',
      isCorrect: false
    }
    setQuestions(questions.map(q => 
      q.id === questionId ? { ...q, answers: [...q.answers, newAnswer] } : q
    ))
  }

  const deleteAnswer = (questionId: string, answerId: string) => {
    setQuestions(questions.map(q => 
      q.id === questionId 
        ? { ...q, answers: q.answers.filter(a => a.id !== answerId) }
        : q
    ))
  }

  const updateAnswerText = (questionId: string, answerId: string, text: string) => {
    setQuestions(questions.map(q => 
      q.id === questionId 
        ? {
            ...q,
            answers: q.answers.map(a => 
              a.id === answerId ? { ...a, text } : a
            )
          }
        : q
    ))
  }

  const toggleCorrectAnswer = (questionId: string, answerId: string) => {
    setQuestions(questions.map(q => 
      q.id === questionId 
        ? {
            ...q,
            answers: q.answers.map(a => 
              // Only one answer can be correct, so uncheck others when checking a new one
              a.id === answerId ? { ...a, isCorrect: !a.isCorrect } : { ...a, isCorrect: false }
            )
          }
        : q
    ))
  }

  const handleSaveChanges = () => {
    if (!editingGame) return

    // Validate all questions have exactly 4 answers with one correct
    for (const question of questions) {
      if (question.answers.length !== 4) {
        alert(`Each question must have exactly 4 answers. "${question.text || 'Untitled question'}" has ${question.answers.length}.`)
        return
      }
      
      const correctAnswers = question.answers.filter(a => a.isCorrect)
      if (correctAnswers.length !== 1) {
        alert(`Each question must have exactly one correct answer. "${question.text || 'Untitled question'}" has ${correctAnswers.length}.`)
        return
      }
    }

    const updatedGame: GameData = {
      ...editingGame,
      name: gameName,
      questions: questions
    }

    const games = JSON.parse(localStorage.getItem('allGames') || '{}')
    games[editingGame.roomCode] = updatedGame
    localStorage.setItem('allGames', JSON.stringify(games))

    alert('Game updated successfully!')
    loadGames()
  }

  const handleDeleteGame = (roomCode: string) => {
    if (confirm('Are you sure you want to delete this game? This action cannot be undone.')) {
      const games = JSON.parse(localStorage.getItem('allGames') || '{}')
      delete games[roomCode]
      localStorage.setItem('allGames', JSON.stringify(games))
      
      // Clean up related data
      localStorage.removeItem(`players_${roomCode}`)
      localStorage.removeItem(`game_${roomCode}_started`)
      
      loadGames()
      if (editingGame?.roomCode === roomCode) {
        handleBackToList()
      }
    }
  }

  // Game List View
  if (!editingGame) {
    const gamesList = Object.values(allGames)

    return (
      <section className="min-h-screen px-4 py-16">
        <div className="mx-auto max-w-4xl">
          <div className="mb-8">
            <h1 className="mb-2 text-3xl font-bold">Edit Games</h1>
            <p className="text-muted-foreground">
              Select a game to edit or delete
            </p>
          </div>

          {gamesList.length === 0 ? (
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
              {gamesList.map((game) => (
                <Card key={game.roomCode}>
                  <CardHeader>
                    <div className="flex items-start justify-between">
                      <div>
                        <CardTitle className="text-xl">{game.name}</CardTitle>
                        <CardDescription className="mt-1">
                          Room Code: {game.roomCode} • {game.questions.length} questions
                        </CardDescription>
                        <CardDescription className="mt-1 text-xs">
                          Created: {new Date(game.createdAt).toLocaleDateString()}
                        </CardDescription>
                      </div>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleSelectGame(game.roomCode)}
                        >
                          <Edit className="mr-2 h-4 w-4" />
                          Edit
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleDeleteGame(game.roomCode)}
                        >
                          <Trash2 className="h-4 w-4 text-red-600" />
                        </Button>
                      </div>
                    </div>
                  </CardHeader>
                </Card>
              ))}
            </div>
          )}
        </div>
      </section>
    )
  }

  // Edit Game View
  return (
    <section className="min-h-screen px-4 py-16">
      <div className="mx-auto max-w-4xl">
        <div className="mb-8">
          <Button
            variant="ghost"
            onClick={handleBackToList}
            className="mb-4"
          >
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Games List
          </Button>
          <h1 className="mb-2 text-3xl font-bold">Edit Game</h1>
          <p className="text-muted-foreground">
            Modify questions and answers for your game
          </p>
        </div>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Game Details</CardTitle>
            <CardDescription>
              Room Code: {editingGame.roomCode}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              <Label htmlFor="gameName">Game Name</Label>
              <Input
                id="gameName"
                placeholder="Enter game name..."
                value={gameName}
                onChange={(e) => setGameName(e.target.value)}
              />
            </div>
          </CardContent>
        </Card>

        <div className="mb-6 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-semibold">Questions</h2>
            <Button onClick={addQuestion} size="sm">
              <Plus className="mr-2 h-4 w-4" />
              Add Question
            </Button>
          </div>

          {questions.length === 0 && (
            <Card>
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">
                  {"No questions yet. Click \"Add Question\" to get started."}
                </p>
              </CardContent>
            </Card>
          )}

          {questions.map((question, qIndex) => (
            <Card key={question.id}>
              <CardHeader>
                <div className="flex items-start justify-between">
                  <CardTitle className="text-lg">Question {qIndex + 1}</CardTitle>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => deleteQuestion(question.id)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor={`question-${question.id}`}>Question Text</Label>
                  <Textarea
                    id={`question-${question.id}`}
                    placeholder="Enter your question..."
                    value={question.text}
                    onChange={(e) => updateQuestionText(question.id, e.target.value)}
                    rows={3}
                  />
                </div>

                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <Label>Answers ({question.answers.length}/4)</Label>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => addAnswer(question.id)}
                      disabled={question.answers.length >= 4}
                    >
                      <Plus className="mr-2 h-4 w-4" />
                      Add Answer
                    </Button>
                  </div>

                  {question.answers.length === 0 && (
                    <p className="text-muted-foreground text-sm">
                      No answers yet. Add exactly 4 answers with one correct.
                    </p>
                  )}

                  {question.answers.map((answer, aIndex) => (
                    <div
                      key={answer.id}
                      className="flex items-start gap-2 rounded-lg border p-3"
                    >
                      <div className="flex-1 space-y-2">
                        <Input
                          placeholder={`Answer ${aIndex + 1}...`}
                          value={answer.text}
                          onChange={(e) =>
                            updateAnswerText(question.id, answer.id, e.target.value)
                          }
                        />
                        <label className="flex items-center gap-2 text-sm">
                          <input
                            type="checkbox"
                            checked={answer.isCorrect}
                            onChange={() => toggleCorrectAnswer(question.id, answer.id)}
                            className="h-4 w-4 rounded border-gray-300"
                          />
                          <span className={answer.isCorrect ? 'font-semibold text-green-600' : ''}>
                            Correct Answer
                          </span>
                        </label>
                      </div>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => deleteAnswer(question.id, answer.id)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>

        <div className="flex justify-end gap-3">
          <Button variant="outline" onClick={handleBackToList}>
            Cancel
          </Button>
          <Button 
            onClick={handleSaveChanges} 
            disabled={!gameName || questions.length === 0}
          >
            Save Changes
          </Button>
        </div>
      </div>
    </section>
  )
}
