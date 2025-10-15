'use client'

import { useState, useEffect, Suspense } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { CheckCircle, XCircle, Trophy } from 'lucide-react'

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

interface Player {
  id: string
  name: string
  isHost: boolean
}

export default function GameActivePage() {
  return (
    <Suspense fallback={null}>
      <GameActiveContent />
    </Suspense>
  )
}

function GameActiveContent() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const roomCode = searchParams.get('room')
  
  const [gameData, setGameData] = useState<GameData | null>(null)
  const [currentPlayer, setCurrentPlayer] = useState<Player | null>(null)
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0)
  const [selectedAnswers, setSelectedAnswers] = useState<Record<string, string>>({})
  const [showResults, setShowResults] = useState(false)
  const [gameFinished, setGameFinished] = useState(false)

  useEffect(() => {
    if (!roomCode) {
      router.push('/join-game')
      return
    }

    // Load game data
    const allGames = JSON.parse(localStorage.getItem('allGames') || '{}')
    const game = allGames[roomCode]

    if (!game) {
      router.push('/join-game')
      return
    }

    setGameData(game)

    // Load current player
    const playerData = JSON.parse(localStorage.getItem('currentPlayer') || 'null')
    if (playerData && playerData.roomCode === roomCode) {
      setCurrentPlayer(playerData)
    } else {
      router.push('/join-game')
    }
  }, [roomCode, router])

  const handleSelectAnswer = (questionId: string, answerId: string) => {
    if (!showResults) {
      setSelectedAnswers({
        ...selectedAnswers,
        [questionId]: answerId
      })
    }
  }

  const handleSubmitAnswer = () => {
    setShowResults(true)
  }

  const handleNextQuestion = () => {
    if (gameData && currentQuestionIndex < gameData.questions.length - 1) {
      setCurrentQuestionIndex(currentQuestionIndex + 1)
      setShowResults(false)
    }
  }

  const handleFinishGame = () => {
    setGameFinished(true)
  }

  const calculateScore = () => {
    if (!gameData) return { correct: 0, total: 0, percentage: 0 }
    
    let correct = 0
    gameData.questions.forEach((question) => {
      const selectedAnswerId = selectedAnswers[question.id]
      const selectedAnswer = question.answers.find(a => a.id === selectedAnswerId)
      if (selectedAnswer?.isCorrect) {
        correct++
      }
    })
    
    const total = gameData.questions.length
    const percentage = total > 0 ? Math.round((correct / total) * 100) : 0
    
    return { correct, total, percentage }
  }

  const handlePlayAgain = () => {
    setCurrentQuestionIndex(0)
    setSelectedAnswers({})
    setShowResults(false)
    setGameFinished(false)
  }

  const handleExitGame = () => {
    if (roomCode && currentPlayer) {
      // Remove player from players list
      const storedPlayers = JSON.parse(localStorage.getItem(`players_${roomCode}`) || '[]')
      const updatedPlayers = storedPlayers.filter((p: Player) => p.id !== currentPlayer.id)
      localStorage.setItem(`players_${roomCode}`, JSON.stringify(updatedPlayers))
      localStorage.removeItem('currentPlayer')
    }
    router.push('/join-game')
  }

  if (!gameData || !currentPlayer) {
    return null
  }

  const currentQuestion = gameData.questions[currentQuestionIndex]
  const selectedAnswerId = currentQuestion ? selectedAnswers[currentQuestion.id] : null
  const isLastQuestion = currentQuestionIndex === gameData.questions.length - 1

  if (gameFinished) {
    return (
      <section className="flex min-h-screen items-center justify-center bg-zinc-50 px-4 py-16 dark:bg-transparent">
        <div className="w-full max-w-2xl">
          <Card>
            <CardHeader className="text-center">
              <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-yellow-100 dark:bg-yellow-900/20">
                <Trophy className="h-8 w-8 text-yellow-600 dark:text-yellow-400" />
              </div>
              <CardTitle className="text-3xl">Game Complete!</CardTitle>
              <CardDescription>{gameData.name}</CardDescription>
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
                {gameData.questions.map((question, index) => {
                  const selectedAnswerId = selectedAnswers[question.id]
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
    <section className="flex min-h-screen items-center justify-center bg-zinc-50 px-4 py-16 dark:bg-transparent">
      <div className="w-full max-w-2xl">
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardDescription>
                Question {currentQuestionIndex + 1} of {gameData.questions.length}
              </CardDescription>
              <CardDescription>
                Room: {gameData.roomCode}
              </CardDescription>
            </div>
            <CardTitle className="text-2xl">{currentQuestion.text}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="space-y-3">
              {currentQuestion.answers.map((answer) => {
                const isSelected = selectedAnswerId === answer.id
                const showCorrect = showResults && answer.isCorrect
                const showIncorrect = showResults && isSelected && !answer.isCorrect
                
                return (
                  <button
                    key={answer.id}
                    onClick={() => handleSelectAnswer(currentQuestion.id, answer.id)}
                    disabled={showResults}
                    className={`w-full rounded-lg border-2 p-4 text-left transition-all ${
                      showCorrect
                        ? 'border-green-500 bg-green-50 dark:bg-green-900/20'
                        : showIncorrect
                        ? 'border-red-500 bg-red-50 dark:bg-red-900/20'
                        : isSelected
                        ? 'border-blue-500 bg-blue-50 dark:bg-blue-900/20'
                        : 'border-gray-200 hover:border-gray-300 dark:border-gray-700'
                    } ${showResults ? 'cursor-not-allowed' : 'cursor-pointer'}`}
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

            {!showResults ? (
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
