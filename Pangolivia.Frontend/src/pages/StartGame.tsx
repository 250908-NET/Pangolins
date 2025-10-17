import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Play, Users, Calendar } from 'lucide-react'

interface GameData {
  roomCode: string
  name: string
  questions: unknown[]
  createdAt: string
}

export default function StartGamePage() {
  const navigate = useNavigate()
  const [allGames, setAllGames] = useState<Record<string, GameData>>({})
  const [hostName, setHostName] = useState('')
  const [selectedGame, setSelectedGame] = useState<string | null>(null)

  useEffect(() => {
    loadGames()
  }, [])

  const loadGames = () => {
    const games = JSON.parse(localStorage.getItem('allGames') || '{}')
    setAllGames(games)
  }

  const handleStartGame = (roomCode: string) => {
    if (!hostName.trim()) {
      alert('Please enter your name before starting the game')
      return
    }

    // Create host player
    const hostPlayer = {
      id: crypto.randomUUID(),
      name: hostName,
      roomCode: roomCode,
      isHost: true,
      joinedAt: new Date().toISOString()
    }
    
    // Store host as current player
    localStorage.setItem('currentPlayer', JSON.stringify(hostPlayer))
    
    // Initialize players list with host
    localStorage.setItem(`players_${roomCode}`, JSON.stringify([hostPlayer]))
    
    // Redirect to lobby
    navigate(`/game-lobby?room=${roomCode}`)
  }

  const gamesList = Object.values(allGames)

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
            <h2 className="text-xl font-semibold">Available Games</h2>
            {gamesList.map((game) => (
              <Card 
                key={game.roomCode}
                className={`cursor-pointer transition-all ${
                  selectedGame === game.roomCode 
                    ? 'border-blue-500 ring-2 ring-blue-200 dark:ring-blue-900' 
                    : 'hover:border-gray-400'
                }`}
                onClick={() => setSelectedGame(game.roomCode)}
              >
                <CardHeader>
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <CardTitle className="text-xl">{game.name}</CardTitle>
                      <div className="mt-2 flex flex-wrap gap-3 text-sm text-muted-foreground">
                        <div className="flex items-center gap-1">
                          <Users className="h-4 w-4" />
                          <span>{game.questions.length} questions</span>
                        </div>
                        <div className="flex items-center gap-1">
                          <Calendar className="h-4 w-4" />
                          <span>Created {new Date(game.createdAt).toLocaleDateString()}</span>
                        </div>
                      </div>
                      <CardDescription className="mt-2">
                        Room Code: <span className="font-mono font-semibold">{game.roomCode}</span>
                      </CardDescription>
                    </div>
                    <Button
                      onClick={(e) => {
                        e.stopPropagation()
                        handleStartGame(game.roomCode)
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
