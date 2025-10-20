import { useReducer, useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { CheckCircle, XCircle, Trophy, Loader2 } from 'lucide-react'
import { useQuiz } from '@/hooks/useQuizzes'

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

const PREFILL_QUIZ_ID = 1; // Development: prefill with a valid quiz ID

interface Player {
  id: string
  name: string
  isHost: boolean
}

interface GameState {
  currentPlayer: Player | null
  currentQuestionIndex: number
  selectedAnswers: Record<string, string>
  showResults: boolean
  gameFinished: boolean
  questions: Question[]
}

type GameAction =
  | { type: 'SET_CURRENT_PLAYER'; payload: Player | null }
  | { type: 'SET_QUESTIONS'; payload: Question[] }
  | { type: 'SELECT_ANSWER'; payload: { questionId: string; answerId: string } }
  | { type: 'SUBMIT_ANSWER' }
  | { type: 'NEXT_QUESTION' }
  | { type: 'FINISH_GAME' }
  | { type: 'PLAY_AGAIN' }

function gameReducer(state: GameState, action: GameAction): GameState {
  switch (action.type) {
    case 'SET_CURRENT_PLAYER':
      return { ...state, currentPlayer: action.payload }
    
    case 'SET_QUESTIONS':
      return { ...state, questions: action.payload }
    
    case 'SELECT_ANSWER':
      if (state.showResults) return state
      return {
        ...state,
        selectedAnswers: {
          ...state.selectedAnswers,
          [action.payload.questionId]: action.payload.answerId,
        },
      }
    
    case 'SUBMIT_ANSWER':
      return { ...state, showResults: true }
    
    case 'NEXT_QUESTION':
      if (state.questions.length === 0 || state.currentQuestionIndex >= state.questions.length - 1) {
        return state
      }
      return {
        ...state,
        currentQuestionIndex: state.currentQuestionIndex + 1,
        showResults: false,
      }
    
    case 'FINISH_GAME':
      return { ...state, gameFinished: true }
    
    case 'PLAY_AGAIN':
      return {
        ...state,
        currentQuestionIndex: 0,
        selectedAnswers: {},
        showResults: false,
        gameFinished: false,
      }
    
    default:
      return state
  }
}

const initialGameState: GameState = {
  currentPlayer: null,
  currentQuestionIndex: 0,
  selectedAnswers: {},
  showResults: false,
  gameFinished: false,
  questions: [],
}

export default function GameActivePage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const quizIdParam = searchParams.get('quiz')
  const quizId = quizIdParam ? parseInt(quizIdParam) : PREFILL_QUIZ_ID
  
  const { data: quiz, isLoading: loadingQuiz } = useQuiz(quizId)
  const [state, dispatch] = useReducer(gameReducer, initialGameState)

  // Transform API quiz data to local Question format
  useEffect(() => {
    if (quiz) {
      const transformedQuestions: Question[] = quiz.questions.map((q) => ({
        id: q.id.toString(),
        text: q.questionText,
        answers: q.options.map((opt, idx) => ({
          id: `${q.id}-${idx}`,
          text: opt,
          isCorrect: idx === q.correctOptionIndex,
        })),
      }))
      dispatch({ type: 'SET_QUESTIONS', payload: transformedQuestions })
    }
  }, [quiz])

  useEffect(() => {
    if (!quizId) {
      navigate('/join-game')
      return
    }

    // Load current player
    const playerData = JSON.parse(localStorage.getItem('currentPlayer') || 'null')
    if (playerData && playerData.quizId === quizId) {
      dispatch({ type: 'SET_CURRENT_PLAYER', payload: playerData })
    } else {
      navigate('/join-game')
    }
  }, [quizId, navigate])

  const handleSelectAnswer = (questionId: string, answerId: string) => {
    dispatch({ type: 'SELECT_ANSWER', payload: { questionId, answerId } })
  }

  const handleSubmitAnswer = () => {
    dispatch({ type: 'SUBMIT_ANSWER' })
  }

  const handleNextQuestion = () => {
    dispatch({ type: 'NEXT_QUESTION' })
  }

  const handleFinishGame = () => {
    dispatch({ type: 'FINISH_GAME' })
  }

  const calculateScore = () => {
    if (state.questions.length === 0) return { correct: 0, total: 0, percentage: 0 }
    
    let correct = 0
    state.questions.forEach((question) => {
      const selectedAnswerId = state.selectedAnswers[question.id]
      const selectedAnswer = question.answers.find(a => a.id === selectedAnswerId)
      if (selectedAnswer?.isCorrect) {
        correct++
      }
    })
    
    const total = state.questions.length
    const percentage = total > 0 ? Math.round((correct / total) * 100) : 0
    
    return { correct, total, percentage }
  }

  const handlePlayAgain = () => {
    dispatch({ type: 'PLAY_AGAIN' })
  }

  const handleExitGame = () => {
    if (quizId && state.currentPlayer) {
      // Remove player from players list
      const storedPlayers = JSON.parse(localStorage.getItem(`players_${quizId}`) || '[]')
      const updatedPlayers = storedPlayers.filter((p: Player) => p.id !== state.currentPlayer!.id)
      localStorage.setItem(`players_${quizId}`, JSON.stringify(updatedPlayers))
      localStorage.removeItem('currentPlayer')
    }
    navigate('/join-game')
  }

  if (loadingQuiz) {
    return (
      <section className="flex min-h-[calc(100vh-5rem)] items-center justify-center px-4">
        <div className="flex items-center gap-2">
          <Loader2 className="h-8 w-8 animate-spin" />
          <span className="text-lg">Loading quiz...</span>
        </div>
      </section>
    )
  }

  if (!quiz || !state.currentPlayer || state.questions.length === 0) {
    return null
  }

  const currentQuestion = state.questions[state.currentQuestionIndex]
  const selectedAnswerId = currentQuestion ? state.selectedAnswers[currentQuestion.id] : null
  const isLastQuestion = state.currentQuestionIndex === state.questions.length - 1

  if (state.gameFinished) {
    return (
      <section className="flex min-h-[calc(100vh-5rem)] items-center justify-center px-4">
        <div className="w-full max-w-2xl">
          <Card>
            <CardHeader className="text-center">
              <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-yellow-100 dark:bg-yellow-900/20">
                <Trophy className="h-8 w-8 text-yellow-600 dark:text-yellow-400" />
              </div>
              <CardTitle className="text-3xl">Game Complete!</CardTitle>
              <CardDescription>{quiz.quizName}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="text-center">
                <p className="text-6xl font-bold text-blue-600 dark:text-blue-400">
                  {calculateScore().percentage}%
                </p>
                <p className="text-muted-foreground mt-2 text-lg">
                  {calculateScore().correct} out of {calculateScore().total} correct
                </p>
              </div>

              <div className="space-y-3">
                {state.questions.map((question, index) => {
                  const selectedAnswerId = state.selectedAnswers[question.id]
                  const selectedAnswer = question.answers.find(a => a.id === selectedAnswerId)
                  const isCorrect = selectedAnswer?.isCorrect
                  
                  return (
                    <div
                      key={question.id}
                      className={`rounded-lg border p-3 ${
                        isCorrect
                          ? 'border-green-500 bg-green-50 dark:bg-green-900/20'
                          : 'border-red-500 bg-red-50 dark:bg-red-900/20'
                      }`}
                    >
                      <div className="flex items-start gap-2">
                        {isCorrect ? (
                          <CheckCircle className="mt-0.5 h-5 w-5 flex-shrink-0 text-green-600 dark:text-green-400" />
                        ) : (
                          <XCircle className="mt-0.5 h-5 w-5 flex-shrink-0 text-red-600 dark:text-red-400" />
                        )}
                        <div className="flex-1">
                          <p className="text-sm font-medium">
                            Question {index + 1}: {question.text}
                          </p>
                          <p className="text-muted-foreground mt-1 text-xs">
                            Your answer: {selectedAnswer?.text || 'Not answered'}
                          </p>
                        </div>
                      </div>
                    </div>
                  )
                })}
              </div>

              <div className="flex gap-3">
                <Button
                  variant="outline"
                  onClick={handleExitGame}
                  className="flex-1"
                >
                  Exit Game
                </Button>
                <Button onClick={handlePlayAgain} className="flex-1">
                  Play Again
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </section>
    )
  }

  return (
    <section className="flex min-h-[calc(100vh-5rem)] items-center justify-center px-4">
      <div className="w-full max-w-2xl">
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardDescription>
                Question {state.currentQuestionIndex + 1} of {state.questions.length}
              </CardDescription>
              <CardDescription>
                Quiz ID: {quizId}
              </CardDescription>
            </div>
            <CardTitle className="text-2xl">{currentQuestion.text}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="space-y-3">
              {currentQuestion.answers.map((answer) => {
                const isSelected = selectedAnswerId === answer.id
                const showCorrect = state.showResults && answer.isCorrect
                const showIncorrect = state.showResults && isSelected && !answer.isCorrect
                
                return (
                  <button
                    key={answer.id}
                    onClick={() => handleSelectAnswer(currentQuestion.id, answer.id)}
                    disabled={state.showResults}
                    className={`w-full rounded-lg border-2 p-4 text-left transition-all ${
                      showCorrect
                        ? 'border-green-500 bg-green-50 dark:bg-green-900/20'
                        : showIncorrect
                        ? 'border-red-500 bg-red-50 dark:bg-red-900/20'
                        : isSelected
                        ? 'border-blue-500 bg-blue-50 dark:bg-blue-900/20'
                        : 'border-gray-200 hover:border-gray-300 dark:border-gray-700'
                    } ${state.showResults ? 'cursor-not-allowed' : 'cursor-pointer'}`}
                  >
                    <div className="flex items-center justify-between">
                      <span className="font-medium">{answer.text}</span>
                      {showCorrect && (
                        <CheckCircle className="h-5 w-5 text-green-600 dark:text-green-400" />
                      )}
                      {showIncorrect && (
                        <XCircle className="h-5 w-5 text-red-600 dark:text-red-400" />
                      )}
                    </div>
                  </button>
                )
              })}
            </div>

            {!state.showResults ? (
              <Button
                onClick={handleSubmitAnswer}
                disabled={!selectedAnswerId}
                className="w-full"
                size="lg"
              >
                Submit Answer
              </Button>
            ) : (
              <div className="space-y-3">
                {currentQuestion.answers.find(a => a.id === selectedAnswerId)?.isCorrect ? (
                  <div className="flex items-center gap-2 rounded-lg bg-green-50 p-3 dark:bg-green-900/20">
                    <CheckCircle className="h-5 w-5 text-green-600 dark:text-green-400" />
                    <p className="font-medium text-green-600 dark:text-green-400">
                      Correct!
                    </p>
                  </div>
                ) : (
                  <div className="flex items-center gap-2 rounded-lg bg-red-50 p-3 dark:bg-red-900/20">
                    <XCircle className="h-5 w-5 text-red-600 dark:text-red-400" />
                    <p className="font-medium text-red-600 dark:text-red-400">
                      Incorrect
                    </p>
                  </div>
                )}
                
                {isLastQuestion ? (
                  <Button onClick={handleFinishGame} className="w-full" size="lg">
                    View Results
                  </Button>
                ) : (
                  <Button onClick={handleNextQuestion} className="w-full" size="lg">
                    Next Question
                  </Button>
                )}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </section>
  )
}
