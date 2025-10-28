import { Link } from 'react-router-dom'
import { FaHome } from 'react-icons/fa'
import { FaMoon, FaSun } from 'react-icons/fa'
// Simple theme toggle using document root class
const useTheme = () => {
    const [theme, setTheme] = React.useState(() => {
        // Check localStorage first, then fallback to checking classList
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme) {
            return savedTheme;
        }
        return document.documentElement.classList.contains('dark') ? 'dark' : 'light';
    });

    const toggleTheme = () => {
        const newTheme = theme === 'light' ? 'dark' : 'light';
        // Update DOM
        if (newTheme === 'dark') {
            document.documentElement.classList.add('dark');
        } else {
            document.documentElement.classList.remove('dark');
        }
        // Update state and localStorage
        setTheme(newTheme);
        localStorage.setItem('theme', newTheme);
    };

    // Sync initial theme with DOM on mount
    React.useEffect(() => {
        if (theme === 'dark') {
            document.documentElement.classList.add('dark');
        } else {
            document.documentElement.classList.remove('dark');
        }
    }, []);

    return { theme, toggleTheme };
};
import { Menu, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import React from 'react'
import { cn } from '@/lib/utils'
import { useAuth } from '@/hooks/useAuth'

const menuItems = [
    { name: 'Start Game', href: '/start-game' },
    { name: 'Create Quiz', href: '/create-game' },
    { name: 'Edit Quiz', href: '/edit-game' },
    { name: 'Join Game', href: '/join-game' },
    { name: 'Profile', href: '/profile' },
]


export const Header = () => {
    const { isAuthenticated, logout } = useAuth();
    const [menuState, setMenuState] = React.useState(false);
    const [isScrolled, setIsScrolled] = React.useState(false);
    const { theme, toggleTheme } = useTheme();

    const handleMenuItemClick = () => {
        setMenuState(false);
    };

    React.useEffect(() => {
        const handleScroll = () => {
            setIsScrolled(window.scrollY > 50);
        };
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);
    return (
        <header role="banner">
            <nav
                aria-label="Main navigation"
                data-state={menuState && 'active'}
                className="fixed z-20 w-full px-2 backdrop-blur-sm">
                <div className={cn('mx-auto mt-2 max-w-6xl px-6 transition-all duration-300 lg:px-12', isScrolled && 'bg-background/50 max-w-4xl rounded-2xl border backdrop-blur-lg lg:px-5')}>
                    <div className="relative flex flex-wrap items-center justify-between gap-6 py-3 lg:gap-0 lg:py-4">
                        <div className="flex w-full justify-between lg:w-auto">
                            <Link
                                to="/"
                                aria-label="Pangolivia home"
                                className="flex items-center space-x-2">
                                <FaHome size={28} aria-hidden="true" />
                            </Link>
                            {/* Theme toggle button */}
                            <button
                                aria-label={`Switch to ${theme === 'dark' ? 'light' : 'dark'} mode`}
                                onClick={toggleTheme}
                                className="ml-2 p-2 rounded-full border border-muted-foreground hover:bg-muted-foreground/10 focus:outline-none focus:ring-2 focus:ring-accent">
                                {theme === 'dark' ? <FaSun size={20} aria-hidden="true" /> : <FaMoon size={20} aria-hidden="true" />}
                            </button>

                            <button
                                onClick={() => setMenuState(!menuState)}
                                aria-label="Navigation menu"
                                aria-expanded={menuState}
                                aria-controls="mobile-nav-menu"
                                className="relative z-20 -m-2.5 -mr-4 block cursor-pointer p-2.5 lg:hidden">
                                <Menu className="in-data-[state=active]:rotate-180 in-data-[state=active]:scale-0 in-data-[state=active]:opacity-0 m-auto size-6 duration-200" aria-hidden="true" />
                                <X className="in-data-[state=active]:rotate-0 in-data-[state=active]:scale-100 in-data-[state=active]:opacity-100 absolute inset-0 m-auto size-6 -rotate-180 scale-0 opacity-0 duration-200" aria-hidden="true" />
                            </button>
                        </div>

                        <div className="absolute inset-0 m-auto hidden size-fit lg:block">
                            <ul className="flex gap-8 text-sm" role="list">
                                {menuItems.map((item, index) => (
                                    <li key={index}>
                                        <Link
                                            to={item.href}
                                            className="text-muted-foreground hover:text-accent-foreground block duration-150">
                                            <span>{item.name}</span>
                                        </Link>
                                    </li>
                                ))}
                            </ul>
                        </div>

                        <div id="mobile-nav-menu" className="bg-background in-data-[state=active]:block lg:in-data-[state=active]:flex mb-6 hidden w-full flex-wrap items-center justify-end space-y-8 rounded-3xl border p-6 shadow-2xl shadow-zinc-300/20 md:flex-nowrap lg:m-0 lg:flex lg:w-fit lg:gap-6 lg:space-y-0 lg:border-transparent lg:bg-transparent lg:p-0 lg:shadow-none dark:shadow-none dark:lg:bg-transparent">
                            <div className="lg:hidden">
                                <ul className="space-y-6 text-base" role="list">
                                    {menuItems.map((item, index) => (
                                        <li key={index}>
                                            <Link
                                                to={item.href}
                                                onClick={handleMenuItemClick}
                                                className="text-muted-foreground hover:text-accent-foreground block duration-150">
                                                <span>{item.name}</span>
                                            </Link>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                            <div className="flex w-full flex-col space-y-3 sm:flex-row sm:gap-3 sm:space-y-0 md:w-fit">
                                {isAuthenticated ? (
                                    <>
                                        <Button onClick={() => { logout(); handleMenuItemClick(); }} variant="outline" size="sm">
                                            <span>Logout</span>
                                        </Button>
                                    </>
                                ) : (
                                    <>
                                        <Button
                                            asChild
                                            variant="outline"
                                            size="sm">
                                            <Link to="/login" onClick={handleMenuItemClick}>
                                                <span>Login</span>
                                            </Link>
                                        </Button>
                                        <Button
                                            asChild
                                            size="sm">
                                            <Link to="/sign-up" onClick={handleMenuItemClick}>
                                                <span>Sign Up</span>
                                            </Link>
                                        </Button>
                                    </>
                                )}
                                {/* <Button
                                    asChild
                                    variant="outline"
                                    size="sm"
                                    className={cn(isScrolled && 'lg:hidden')}>
                                    <Link to="/login">
                                        <span>Login</span>
                                    </Link>
                                </Button>
                                <Button
                                    asChild
                                    size="sm"
                                    className={cn(isScrolled && 'lg:hidden')}>
                                    <Link to="/sign-up">
                                        <span>Sign Up</span>
                                    </Link>
                                </Button>
                                <Button
                                    asChild
                                    size="sm"
                                    className={cn(isScrolled ? 'lg:inline-flex' : 'hidden')}>
                                    <Link to="#">
                                        <span>Get Started</span> 
                                    </Link>
                                </Button> */}
                            </div>
                        </div>
                    </div>
                </div>
            </nav>
        </header>
    );
}
