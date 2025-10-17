import { createContext, useState, useEffect, useContext } from 'react'
import { useNavigate } from 'react-router-dom'
import { jwtDecode } from 'jwt-decode'

interface User {
  name: string
}

interface AuthContextType {
  isAuthenticated: boolean
  user: User | null
  login: (username: string, password: string) => Promise<void>
  register: (username: string, password: string) => Promise<void>
  logout: () => void
  loading: boolean
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

const API_URL = 'http://localhost:3001/api/Auth'

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'))
  const [loading, setLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    if (token) {
      try {
        const decoded: { name: string; exp: number } = jwtDecode(token)
        // Check if token is expired
        if (decoded.exp * 1000 > Date.now()) {
          setUser({ name: decoded.name })
        } else {
          // Token expired, clear it
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
  }, [token])

  const login = async (username: string, password: string) => {
    const response = await fetch(`${API_URL}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    })

    if (!response.ok) {
      const errorData = await response.json()
      throw new Error(errorData.message || 'Login failed')
    }

    const data = await response.json()
    localStorage.setItem('token', data.token)
    setToken(data.token)
    const decoded: { name: string } = jwtDecode(data.token)
    setUser({ name: decoded.name })
    navigate('/') // Redirect to home after login
  }

  const register = async (username: string, password: string) => {
    const response = await fetch(`${API_URL}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    })

    if (!response.ok) {
      const errorData = await response.json()
      throw new Error(errorData.message || 'Registration failed')
    }

    navigate('/login')
  }

  const logout = () => {
    localStorage.removeItem('token')
    setUser(null)
    setToken(null)
    navigate('/login') // Redirect to login after logout
  }

  const value = {
    isAuthenticated: !!user,
    user,
    login,
    register,
    logout,
    loading,
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