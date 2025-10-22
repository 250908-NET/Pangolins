import { createContext, useState, useCallback, useEffect, useRef } from 'react'
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'


export interface Player {
  userId: number
  username: string
  isHost: boolean
}

export interface LobbyDetails {
  quizName: string
  creatorUsername: string
  questionCount: number
  hostUsername: string
}

export interface QuestionForPlayer {
  questionText: string
  answer1: string
  answer2: string
  answer3: string
  answer4: string
}

export interface RoundResults {
  question: string
  answer: string
  playerScores: { userId: number; username: string; score: number }[]
}

export interface FinalGameRecord {
  hostUserId: number
  quizId: number
  playerScores: { userId: number; username: string; score: number }[]
}

// --- Context Type Definition ---

export interface SignalRContextType {
  connection: HubConnection | null
  connectToHub: (hubPath: string) => Promise<void>
  disconnect: () => Promise<void>

  // Game State
  lobbyDetails: LobbyDetails | null
  players: Player[]
  gameStarted: boolean
  currentQuestion: QuestionForPlayer | null
  roundResults: RoundResults | null
  finalResults: FinalGameRecord | null
  answerSubmitted: boolean

  // Actions (functions to call hub methods)
  joinLobby: (roomCode: string) => Promise<void>
  startLobbyGame: (roomCode: string) => Promise<void>
  submitPlayerAnswer: (roomCode: string, answer: string) => Promise<void>
  beginGameLoop: (roomCode: string) => Promise<void> // For host
  skipCurrentQuestion: (roomCode: string) => Promise<void> // For host
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined)

export function SignalRProvider({ children }: { children: React.ReactNode }) {
  const [connection, setConnection] = useState<HubConnection | null>(null)
  const connectionRef = useRef<HubConnection | null>(null) // Ref to hold the current connection

  // --- All Game State Lives Here ---
  const [lobbyDetails, setLobbyDetails] = useState<LobbyDetails | null>(null)
  const [players, setPlayers] = useState<Player[]>([])
  const [gameStarted, setGameStarted] = useState(false)
  const [currentQuestion, setCurrentQuestion] =
    useState<QuestionForPlayer | null>(null)
  const [roundResults, setRoundResults] = useState<RoundResults | null>(null)
  const [finalResults, setFinalResults] = useState<FinalGameRecord | null>(null)
  const [answerSubmitted, setAnswerSubmitted] = useState(false)

  // Reset state when disconnected or game ends
  const resetGameState = useCallback(() => {
    setLobbyDetails(null)
    setPlayers([])
    setGameStarted(false)
    setCurrentQuestion(null)
    setRoundResults(null)
    setFinalResults(null)
    setAnswerSubmitted(false)
  }, [])

  // --- Centralized Event Listeners ---
  useEffect(() => {
    if (!connection) {
      return
    }

    // --- Lobby Listeners ---
    connection.on('receivelobbydetails', (details: LobbyDetails) =>
      setLobbyDetails(details),
    )
    connection.on('updateplayerlist', (playerList: Player[]) =>
      setPlayers(playerList),
    )
    connection.on('gamestarted', () => setGameStarted(true))

    // --- Active Game Listeners ---
    connection.on('receivequestion', (question: QuestionForPlayer) => {
      // Reset round-specific state
      setCurrentQuestion(question)
      setRoundResults(null)
      setAnswerSubmitted(false)
      setFinalResults(null) // Clear final results
    })
    connection.on('receiveroundresults', (results: RoundResults) =>
      setRoundResults(results),
    )
    connection.on('gameended', (finalRecord: FinalGameRecord) =>
      setFinalResults(finalRecord),
    )
    connection.on('answersubmitted', () => setAnswerSubmitted(true))

    // --- Global Listener ---
    connection.on('error', (message: string) => {
      // You can use a toast library here
      console.error('SignalR Hub Error:', message)
    })

    // Cleanup on disconnect
    return () => {
      // We can turn them all off by just rebuilding the connection,
      // but explicit cleanup is safer.
      connection.off('receivelobbydetails')
      connection.off('updateplayerlist')
      connection.off('gamestarted')
      connection.off('receivequestion')
      connection.off('receiveroundresults')
      connection.off('gameended')
      connection.off('answersubmitted')
      connection.off('error')
    }
  }, [connection])

  // --- Connection Management ---
  const connectToHub = useCallback(async (hubPath: string) => {
    try {
      const hubUrl = `${import.meta.env.VITE_API_URL}${hubPath}`
      const newConnection = new HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => localStorage.getItem('token') || '',
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build()

      await newConnection.start()
      connectionRef.current = newConnection // Update the ref
      setConnection(newConnection) // Update state to trigger re-renders
    } catch (e) {
      console.error('SignalR Connection Error: ', e)
    }
  }, [])

  const disconnect = useCallback(async () => {
    if (connectionRef.current) {
      await connectionRef.current.stop()
    }
    connectionRef.current = null
    setConnection(null)
    resetGameState()
  }, [resetGameState])

  // --- Actions to Invoke Hub Methods ---
  const invokeHubMethod = useCallback(
    async (methodName: string, ...args: unknown[]) => {
      if (!connectionRef.current)
        throw new Error('SignalR connection not established.')
      return connectionRef.current.invoke(methodName, ...args)
    },
    [],
  )

  const joinLobby = (roomCode: string) => invokeHubMethod('JoinGame', roomCode)
  const startLobbyGame = (roomCode: string) =>
    invokeHubMethod('StartGame', roomCode)
  const submitPlayerAnswer = (roomCode: string, answer: string) =>
    invokeHubMethod('SubmitAnswer', roomCode, answer)
  const beginGameLoop = (roomCode: string) =>
    invokeHubMethod('BeginGame', roomCode)
  const skipCurrentQuestion = (roomCode: string) =>
    invokeHubMethod('SkipQuestion', roomCode)

  const value = {
    connection,
    connectToHub,
    disconnect,
    lobbyDetails,
    players,
    gameStarted,
    currentQuestion,
    roundResults,
    finalResults,
    answerSubmitted,
    joinLobby,
    startLobbyGame,
    submitPlayerAnswer,
    beginGameLoop,
    skipCurrentQuestion,
  }

  return (
    <SignalRContext.Provider value={value}>{children}</SignalRContext.Provider>
  )
}

export { SignalRContext }