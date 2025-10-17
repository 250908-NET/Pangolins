import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Trash2 } from 'lucide-react'

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

export default function CreateGamePage() {
  const navigate = useNavigate()
  const [gameName, setGameName] = useState('')
  const [questions, setQuestions] = useState<Question[]>([])
  const [hostName, setHostName] = useState('')

  const generateRoomCode = () => {
    return Math.floor(100000 + Math.random() * 900000).toString()
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

  const handleSaveGame = () => {
    if (!hostName.trim()) {
      alert('Please enter your name before creating the game')
      return
    }

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

    const newRoomCode = generateRoomCode()
    const gameData = {
      roomCode: newRoomCode,
      name: gameName,
      questions: questions,
      createdAt: new Date().toISOString()
    }
    
    // Store in localStorage with room code as key
    const allGames = JSON.parse(localStorage.getItem('allGames') || '{}')
    allGames[newRoomCode] = gameData
    localStorage.setItem('allGames', JSON.stringify(allGames))
    
    // Create host player
    const hostPlayer = {
      id: crypto.randomUUID(),
      name: hostName,
      roomCode: newRoomCode,
      isHost: true,
      joinedAt: new Date().toISOString()
    }
    
    // Store host as current player
    localStorage.setItem('currentPlayer', JSON.stringify(hostPlayer))
    
    // Initialize players list with host
    localStorage.setItem(`players_${newRoomCode}`, JSON.stringify([hostPlayer]))
    
    // Redirect to lobby
    navigate(`/game-lobby?room=${newRoomCode}`)
  }

  return (
    <section className="min-h-screen px-4 py-16">
      <div className="mx-auto max-w-4xl">
        <div className="mb-8">
          <h1 className="mb-2 text-3xl font-bold">Create New Game</h1>
          <p className="text-muted-foreground">
            Build your custom trivia game with questions and answers
          </p>
        </div>

        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Game Details</CardTitle>
            <CardDescription>Set up your game information</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="hostName">Your Name (Host)</Label>
              <Input
                id="hostName"
                placeholder="Enter your name..."
                value={hostName}
                onChange={(e) => setHostName(e.target.value)}
              />
            </div>
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
          <Button variant="outline" onClick={() => navigate('/')}>Cancel</Button>
          <Button 
            onClick={handleSaveGame} 
            disabled={!hostName.trim() || !gameName || questions.length === 0}
          >
            Create & Go to Lobby
          </Button>
        </div>
      </div>
    </section>
  )
}
