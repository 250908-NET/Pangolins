import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { Header } from '@/components/header'
import Home from '@/pages/Home'
import Login from '@/pages/Login'
import SignUp from '@/pages/SignUp'
import CreateGame from '@/pages/CreateGame'
import StartGame from '@/pages/StartGame'
import JoinGame from '@/pages/JoinGame'
import EditGame from '@/pages/EditGame'
import GameLobby from '@/pages/GameLobby'
import GameActive from '@/pages/GameActive'
import Profile from '@/pages/Profile'
import NotFound from '@/pages/NotFound'

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-background text-foreground antialiased">
        <Header />
        <main className="pt-6">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/sign-up" element={<SignUp />} />
            <Route path="/create-game" element={<CreateGame />} />
            <Route path="/start-game" element={<StartGame />} />
            <Route path="/join-game" element={<JoinGame />} />
            <Route path="/edit-game" element={<EditGame />} />
            <Route path="/game-lobby" element={<GameLobby />} />
            <Route path="/game-active" element={<GameActive />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </main>
      </div>
    </Router>
  )
}

export default App
