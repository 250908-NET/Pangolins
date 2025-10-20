import { LogoIcon } from '@/components/logo'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useAuth } from '@/contexts/AuthContext'
import { useState, useEffect } from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { AlertCircle } from 'lucide-react'

export default function LoginPage() {
    const navigate = useNavigate()
    const location = useLocation()
    const { login, isAuthenticated } = useAuth()
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('')
    const [loading, setLoading] = useState(false)

    const from = location.state?.from?.pathname || '/'

    useEffect(() => {
        if (isAuthenticated) {
            navigate('/', { replace: true })
        }
    }, [isAuthenticated, navigate])

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError('')
        setLoading(true)
        try {
            await login(username, password)
            navigate(from, { replace: true })
        } catch (err: any) {
            setError(err.message || 'An unexpected error occurred.')
        } finally {
            setLoading(false)
        }
    }

    return (
        <section className="flex min-h-[calc(100vh-5rem)] px-4">
            <form
                onSubmit={handleSubmit}
                className="bg-zinc-900 m-auto h-fit w-full max-w-sm overflow-hidden rounded-[calc(var(--radius)+.125rem)] shadow-md shadow-zinc-950/10 border border-zinc-800">
                <div className="bg-zinc-800 -m-px rounded-[calc(var(--radius)+.125rem)] p-8 pb-6 border border-zinc-700">
                        <div className="text-center text-white">
                        <h1 className="mb-1 mt-4 text-xl font-semibold text-white">Login to Pangolivia</h1>
                        <p className="text-sm text-zinc-200">Welcome back!</p>
                    </div>

                    <div className="mt-6 space-y-6">
                        {error && (
                            <div className="flex items-start gap-2 rounded-lg bg-red-50 p-3 dark:bg-red-900/20">
                                <AlertCircle className="mt-0.5 h-4 w-4 text-red-600 dark:text-red-400" />
                                <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
                            </div>
                        )}
                        <div className="space-y-2">
                            <Label
                                htmlFor="username"
                                className="block text-sm text-white">
                                Username
                            </Label>
                            <Input
                                type="text"
                                required
                                name="username"
                                id="username"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                className="bg-zinc-900 text-white placeholder-zinc-300 border-zinc-700 focus:border-yellow-400"
                                placeholder="Username"
                            />
                        </div>

                        <div className="space-y-0.5">
                            <div className="flex items-center justify-between">
                                <Label
                                    htmlFor="pwd"
                                    className="text-sm text-white">
                                    Password
                                </Label>
                                <Button
                                    asChild
                                    variant="link"
                                    size="sm"
                                    className="text-yellow-400 hover:text-yellow-300 focus:underline">
                                    <Link
                                        to="#"
                                        className="text-yellow-400 hover:text-yellow-300 text-sm">
                                        Forgot your Password ?
                                    </Link>
                                </Button>
                            </div>
                            <Input
                                type="password"
                                required
                                name="pwd"
                                id="pwd"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                className="bg-zinc-900 text-white placeholder-zinc-300 border-zinc-700 focus:border-yellow-400"
                                placeholder="Password"
                            />
                        </div>

                        <Button type="submit" className="w-full bg-yellow-400 text-black font-semibold hover:bg-yellow-500" disabled={loading}>
                            {loading ? 'Logging in...' : 'Login'}
                        </Button>
                    </div>

                    <div className="my-6 grid grid-cols-[1fr_auto_1fr] items-center gap-3">
                        <hr className="border-dashed border-zinc-700" />
                        <span className="text-xs text-zinc-200">Or continue with</span>
                        <hr className="border-dashed border-zinc-700" />
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <Button
                            type="button"
                            variant="outline"
                            className="bg-zinc-900 text-white border-zinc-600 hover:bg-zinc-800 focus:ring-2 focus:ring-yellow-400">
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="0.98em"
                                height="1em"
                                viewBox="0 0 256 262">
                                <path
                                    fill="#4285f4"
                                    d="M255.878 133.451c0-10.734-.871-18.567-2.756-26.69H130.55v48.448h71.947c-1.45 12.04-9.283 30.172-26.69 42.356l-.244 1.622l38.755 30.023l2.685.268c24.659-22.774 38.875-56.282 38.875-96.027"></path>
                                <path
                                    fill="#34a853"
                                    d="M130.55 261.1c35.248 0 64.839-11.605 86.453-31.622l-41.196-31.913c-11.024 7.688-25.82 13.055-45.257 13.055c-34.523 0-63.824-22.773-74.269-54.25l-1.531.13l-40.298 31.187l-.527 1.465C35.393 231.798 79.49 261.1 130.55 261.1"></path>
                                <path
                                    fill="#fbbc05"
                                    d="M56.281 156.37c-2.756-8.123-4.351-16.827-4.351-25.82c0-8.994 1.595-17.697 4.206-25.82l-.073-1.73L15.26 71.312l-1.335.635C5.077 89.644 0 109.517 0 130.55s5.077 40.905 13.925 58.602z"></path>
                                <path
                                    fill="#eb4335"
                                    d="M130.55 50.479c24.514 0 41.05 10.589 50.479 19.438l36.844-35.974C195.245 12.91 165.798 0 130.55 0C79.49 0 35.393 29.301 13.925 71.947l42.211 32.783c10.59-31.477 39.891-54.251 74.414-54.251"></path>
                            </svg>
                            <span className="text-white">Google</span>
                        </Button>
                        <Button
                            type="button"
                            variant="outline"
                            className="bg-zinc-900 text-white border-zinc-600 hover:bg-zinc-800 focus:ring-2 focus:ring-yellow-400">
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                width="1em"
                                height="1em"
                                viewBox="0 0 256 256">
                                <path
                                    fill="#f1511b"
                                    d="M121.666 121.666H0V0h121.666z"></path>
                                <path
                                    fill="#80cc28"
                                    d="M256 121.666H134.335V0H256z"></path>
                                <path
                                    fill="#00adef"
                                    d="M121.663 256.002H0V134.336h121.663z"></path>
                                <path
                                    fill="#fbbc09"
                                    d="M256 256.002H134.335V134.336H256z"></path>
                            </svg>
                            <span className="text-white">Microsoft</span>
                        </Button>
                    </div>
                </div>

                <div className="p-3">
                    <p className="text-center text-sm text-zinc-200">
                        {"Don't have an account ?"} 
                        <Button
                            asChild
                            variant="link"
                            className="px-2 text-yellow-400 hover:text-yellow-300 focus:underline">
                            <Link to="/sign-up">Sign Up</Link>
                        </Button>
                    </p>
                </div>
            </form>
        </section>
    )
}
