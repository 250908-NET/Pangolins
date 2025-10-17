import { useState, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Trash2, Edit, ArrowLeft, Loader2 } from 'lucide-react'
import { useQuizzes, useQuiz, useUpdateQuiz, useDeleteQuiz } from '@/hooks/useQuizzes'
import type { QuestionDto } from '@/types/api'

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

// Hardcoded user ID for now (from mock data)
const CURRENT_USER_ID = 1;

export default function EditGamePage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const editQuizId = searchParams.get('quiz') ? parseInt(searchParams.get('quiz')!) : null

  const { data: allQuizzes, isLoading: loadingQuizzes } = useQuizzes()
  const { data: editingQuiz, isLoading: loadingQuiz } = useQuiz(editQuizId || 0)
  const updateQuiz = useUpdateQuiz()
  const deleteQuiz = useDeleteQuiz()

  const [gameName, setGameName] = useState('')
  const [questions, setQuestions] = useState<Question[]>([])

  useEffect(() => {
    if (editingQuiz) {
      setGameName(editingQuiz.quizName)
      // Convert API questions to local format
      const localQuestions: Question[] = editingQuiz.questions.map(q => ({
        id: q.id.toString(),
        text: q.questionText,
        answers: q.options.map((opt, idx) => ({
          id: `${q.id}-${idx}`,
          text: opt,
          isCorrect: idx === q.correctOptionIndex,
        })),
      }))
      setQuestions(localQuestions)
    }
  }, [editingQuiz])

  const handleSelectGame = (quizId: number) => {
    navigate(`/edit-game?quiz=${quizId}`)
  }

  const handleBackToList = () => {
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

  const handleSaveChanges = async () => {
    if (!editingQuiz) return

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

    try {
      // Convert local questions to API format
      const apiQuestions: QuestionDto[] = questions.map((q) => {
        const correctIndex = q.answers.findIndex(a => a.isCorrect)
        return {
          id: parseInt(q.id) || 0,
          questionText: q.text,
          options: q.answers.map(a => a.text),
          correctOptionIndex: correctIndex,
        }
      })

      await updateQuiz.mutateAsync({
        quizId: editingQuiz.id,
        quiz: {
          quizName: gameName,
          questions: apiQuestions,
        },
        currentUserId: CURRENT_USER_ID,
      })

      alert('Quiz updated successfully!')
      handleBackToList()
    } catch (error) {
      console.error('Failed to update quiz:', error)
      alert('Failed to update quiz. Please try again.')
    }
  }

  const handleDeleteGame = async (quizId: number) => {
    if (confirm('Are you sure you want to delete this quiz? This action cannot be undone.')) {
      try {
        await deleteQuiz.mutateAsync({ id: quizId, currentUserId: CURRENT_USER_ID })
        
        // Clean up related data
        localStorage.removeItem(`players_${quizId}`)
        localStorage.removeItem(`game_${quizId}_started`)
        
        if (editingQuiz?.id === quizId) {
          handleBackToList()
        }
      } catch (error) {
        console.error('Failed to delete quiz:', error)
        alert('Failed to delete quiz. Please try again.')
      }
    }
  }

  // Game List View
  if (!editQuizId) {
    if (loadingQuizzes) {
      return (
        <section className="min-h-screen px-4 py-16">
          <div className="mx-auto max-w-4xl flex items-center justify-center">
            <Loader2 className="h-8 w-8 animate-spin" />
          </div>
        </section>
      )
    }

    return (
      <section className="min-h-screen px-4 py-16">
        <div className="mx-auto max-w-4xl">
          <div className="mb-8">
            <h1 className="mb-2 text-3xl font-bold">Edit Games</h1>
            <p className="text-muted-foreground">
              Select a game to edit or delete
            </p>
          </div>

          {!allQuizzes || allQuizzes.length === 0 ? (
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
              {allQuizzes.map((quiz) => (
                <Card key={quiz.id}>
                  <CardHeader>
                    <div className="flex items-start justify-between">
                      <div>
                        <CardTitle className="text-xl">{quiz.quizName}</CardTitle>
                        <CardDescription className="mt-1">
                          By {quiz.creatorUsername} • {quiz.questionCount} questions
                        </CardDescription>
                      </div>
                      <div className="flex gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleSelectGame(quiz.id)}
                        >
                          <Edit className="mr-2 h-4 w-4" />
                          Edit
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleDeleteGame(quiz.id)}
                          disabled={deleteQuiz.isPending}
                        >
                          {deleteQuiz.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4 text-red-600" />}
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
  if (loadingQuiz) {
    return (
      <section className="min-h-screen px-4 py-16">
        <div className="mx-auto max-w-4xl flex items-center justify-center">
          <Loader2 className="h-8 w-8 animate-spin" />
        </div>
      </section>
    )
  }

  if (!editingQuiz) {
    return (
      <section className="min-h-screen px-4 py-16">
        <div className="mx-auto max-w-4xl">
          <p className="text-muted-foreground">Quiz not found.</p>
          <Button onClick={handleBackToList} className="mt-4">Back to List</Button>
        </div>
      </section>
    )
  }

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
            <CardTitle>Quiz Details</CardTitle>
            <CardDescription>
              Created by: {editingQuiz.creatorUsername}
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
            disabled={!gameName || questions.length === 0 || updateQuiz.isPending}
          >
            {updateQuiz.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {updateQuiz.isPending ? 'Saving...' : 'Save Changes'}
          </Button>
        </div>
      </div>
    </section>
  )
}
