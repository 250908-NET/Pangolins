import Image from 'next/image'

export const Logo = () => {
    return (
        <Image
            src="/logo.png"
            width={50}
            height={50}
            className="w-auto inline-block transition-transform duration-300 hover:-rotate-3"
            alt="Logo"
            priority = {false}
        />
    )
}

export const LogoIcon = () => {
    return (
        <Image
            src="/logo.png"
            width={30}
            height={30}
            className="w-auto"
            alt="Logo"
            priority = {false}
        />
    )
}

export const LogoStroke = () => {
    return (
        <Image
            src="/logo.png"
            width={75}
            height={25}
            className="w-auto"
            alt="Logo"
            priority = {false}
        />  
    )
}
