'use client'
import Link from 'next/link'
import { Menu, X, Home } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { ThemeToggle } from '@/components/theme-toggle'
import React from 'react'
import { cn } from '@/lib/utils'

const menuItems = [
    { name: 'Home', href: '/', icon: Home },
    { name: 'Start Game', href: '/start-game' },
    { name: 'Create Game', href: '/create-game' },
    { name: 'Edit Game', href: '/edit-game' },
    { name: 'Join Game', href: '/join-game' },
    { name: 'Profile', href: '/profile' },
]

export const Header = () => {
    const [menuState, setMenuState] = React.useState(false)
    const [isScrolled, setIsScrolled] = React.useState(false)

    React.useEffect(() => {
        const handleScroll = () => {
            setIsScrolled(window.scrollY > 50)
        }
        window.addEventListener('scroll', handleScroll)
        return () => window.removeEventListener('scroll', handleScroll)
    }, [])
    return (
        <header>
            <nav
                data-state={menuState && 'active'}
                className="fixed z-20 w-full px-2 backdrop-blur-sm">
                <div className={cn('mx-auto mt-2 max-w-6xl px-6 transition-all duration-300 lg:px-12', isScrolled && 'bg-background/50 max-w-4xl rounded-2xl border backdrop-blur-lg lg:px-5')}>
                    <div className="relative flex items-center justify-center py-3 lg:py-4">
                        {/* Mobile menu button - absolute positioned, doesn't affect layout */}
                        <button
                            onClick={() => setMenuState(!menuState)}
                            aria-label={menuState == true ? 'Close Menu' : 'Open Menu'}
                            className="absolute right-2 top-1/2 -translate-y-1/2 z-20 -m-2.5 -mr-4 block cursor-pointer p-2.5 lg:hidden">
                            <Menu className="in-data-[state=active]:rotate-180 in-data-[state=active]:scale-0 in-data-[state=active]:opacity-0 m-auto size-6 duration-200" />
                            <X className="in-data-[state=active]:rotate-0 in-data-[state=active]:scale-100 in-data-[state=active]:opacity-100 absolute inset-0 m-auto size-6 -rotate-180 scale-0 opacity-0 duration-200" />
                        </button>

                        {/* Centered navigation */}
                        <div className="hidden lg:block">
                            <ul className="flex gap-8 text-sm">
                                {menuItems.map((item, index) => (
                                    <li key={index}>
                                        <Link
                                            href={item.href}
                                            className="text-muted-foreground hover:text-accent-foreground block duration-150">
                                            {item.icon ? (
                                                <item.icon className="size-5" />
                                            ) : (
                                                <span>{item.name}</span>
                                            )}
                                        </Link>
                                    </li>
                                ))}
                            </ul>
                        </div>

                        {/* Auth buttons - absolute positioned on desktop */}
                        <div className="absolute right-0 top-1/2 -translate-y-1/2 hidden lg:flex lg:gap-3 lg:items-center">
                            <ThemeToggle />
                            <Button
                                asChild
                                variant="outline"
                                size="sm"
                                className={cn(isScrolled && 'lg:hidden')}>
                                <Link href="/login">
                                    <span>Login</span>
                                </Link>
                            </Button>
                            <Button
                                asChild
                                size="sm"
                                className={cn(isScrolled && 'lg:hidden')}>
                                <Link href="/sign-up">
                                    <span>Sign Up</span>
                                </Link>
                            </Button>
                            <Button
                                asChild
                                size="sm"
                                className={cn(isScrolled ? 'lg:inline-flex' : 'hidden')}>
                                <Link href="#">
                                    <span>Get Started</span> 
                                </Link>
                            </Button>
                        </div>

                        {/* Mobile menu */}
                        <div className="bg-background in-data-[state=active]:block mb-6 hidden w-full flex-wrap items-center justify-center space-y-8 rounded-3xl border p-6 shadow-2xl shadow-zinc-300/20 md:flex-nowrap lg:hidden">
                            <div className="w-full">
                                <ul className="space-y-6 text-base">
                                    {menuItems.map((item, index) => (
                                        <li key={index}>
                                            <Link
                                                href={item.href}
                                                className="text-muted-foreground hover:text-accent-foreground flex items-center gap-2 duration-150">
                                                {item.icon && <item.icon className="size-5" />}
                                                <span>{item.name}</span>
                                            </Link>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                            <div className="flex w-full flex-col space-y-3 sm:flex-row sm:gap-3 sm:space-y-0 md:w-fit">
                                <ThemeToggle />
                                <Button
                                    asChild
                                    variant="outline"
                                    size="sm">
                                    <Link href="/login">
                                        <span>Login</span>
                                    </Link>
                                </Button>
                                <Button
                                    asChild
                                    size="sm">
                                    <Link href="/sign-up">
                                        <span>Sign Up</span>
                                    </Link>
                                </Button>
                            </div>
                        </div>
                    </div>
                </div>
            </nav>
        </header>
    )
}
