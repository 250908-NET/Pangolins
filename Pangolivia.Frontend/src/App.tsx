import { Routes, Route, Outlet } from 'react-router-dom'
import { Header } from '@/components/header'
import Home from '@/pages/Home'
import Login from '@/pages/Login'
import SignUp from '@/pages/SignUp'
import QuizEditor from '@/pages/QuizEditor'
import StartGame from '@/pages/StartGame'
import JoinGame from '@/pages/JoinGame'
import EditGame from '@/pages/EditGame'
import GameLobby from '@/pages/GameLobby'
import GameActive from '@/pages/GameActive'
import Profile from '@/pages/Profile'
import NotFound from '@/pages/NotFound'
import { Toaster } from '@/components/ui/sonner'
import { ProtectedRoute } from '@/components/ProtectedRoute'
import { SessionExpiredModal } from '@/components/SessionExpiredModal'

// Layout component for protected routes
const ProtectedRoutesLayout = () => (
  <ProtectedRoute>
    <Outlet />
  </ProtectedRoute>
)

function App() {
  return (
    <div className="min-h-screen bg-background text-foreground antialiased">
      <Header />
      <main className="pt-20">
        <Routes>
          {/* Public Routes */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/sign-up" element={<SignUp />} />

          {/* Protected Routes */}
          <Route element={<ProtectedRoutesLayout />}>
            <Route path="/create-game" element={<QuizEditor mode="create" />} />
            <Route path="/join-game" element={<JoinGame />} />
            <Route path="/quiz/edit/:id" element={<QuizEditor mode="edit" />} />
            <Route path="/start-game" element={<StartGame />} />
            <Route path="/edit-game" element={<EditGame />} />
            <Route path="/game-lobby" element={<GameLobby />} />
            <Route path="/game-active" element={<GameActive />} />
            <Route path="/profile" element={<Profile />} />
          </Route>

          {/* Catch-all Not Found Route */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
      <SessionExpiredModal />
      <Toaster richColors position="bottom-center" />
    </div>
  )
}

export default App