import { createContext, useState, useEffect, useContext, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { jwtDecode } from 'jwt-decode'
import { api } from '../lib/api'

interface User {
  id: number
  name: string
}

interface AuthContextType {
  isAuthenticated: boolean
  user: User | null
  login: (username: string, password: string) => Promise<void>
  register: (username: string, password: string) => Promise<void>
  logout: () => void
  loading: boolean
  sessionExpired: boolean
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'))
  const [loading, setLoading] = useState(true)
  const [sessionExpired, setSessionExpired] = useState(false)
  const sessionTimeoutRef = useRef<number | null>(null)
  const navigate = useNavigate()

  const clearSessionTimeout = () => {
    if (sessionTimeoutRef.current) {
      clearTimeout(sessionTimeoutRef.current)
      sessionTimeoutRef.current = null
    }
  }

  useEffect(() => {
    clearSessionTimeout()

    if (token) {
      try {
        const decoded: { name: string; exp: number; sub: string } = jwtDecode(token)
        const expirationTime = decoded.exp * 1000

        if (expirationTime > Date.now()) {
          setUser({ name: decoded.name, id: parseInt(decoded.sub) })
          const timeoutDuration = expirationTime - Date.now()
          sessionTimeoutRef.current = window.setTimeout(() => {
            setSessionExpired(true)
          }, timeoutDuration)
        } else {
          // Token already expired
          localStorage.removeItem('token')
          setToken(null)
          setUser(null)
        }
      } catch (error) {
        console.error('Failed to decode token:', error)
        localStorage.removeItem('token')
        setToken(null)
        setUser(null)
      }
    }
    setLoading(false)

    return () => {
      clearSessionTimeout()
    }
  }, [token])

  const login = async (username: string, password: string) => {
    try {
      const response = await api.post('/Auth/login', { username, password })
      const { token } = response.data
      localStorage.setItem('token', token)
      setToken(token)
      // Navigation is now handled by the component calling login
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || 'Login failed'
      throw new Error(errorMessage)
    }
  }

  const register = async (username: string, password: string) => {
    try {
      await api.post('/Auth/register', { username, password })
      navigate('/login')
    } catch (error: any) {
      const errorMessage = error.response?.data?.message || 'Registration failed'
      throw new Error(errorMessage)
    }
  }

  const logout = () => {
    clearSessionTimeout()
    localStorage.removeItem('token')
    setUser(null)
    setToken(null)
    setSessionExpired(false) // Hide modal on logout
    navigate('/login')
  }

  const value = {
    isAuthenticated: !!user,
    user,
    login,
    register,
    logout,
    loading,
    sessionExpired,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}