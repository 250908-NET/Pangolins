import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import "./styles.css";
import { Header } from "@/components/header";
import { Aclonica, Space_Mono } from "next/font/google"
import { ThemeProvider } from "@/components/theme-provider";

const aclonica = Aclonica({ weight: "400", subsets: ["latin"], variable: "--font-sans" })
const spaceMono = Space_Mono({ weight: "400", subsets: ["latin"], variable: "--font-mono" }) 

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Pangolivia",
  description: "Pangolivia is a fast-paced, interactive quiz game that blends classic trivia fun with the power of AI.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          <Header />
          <main className="pt-6">{children}</main>
        </ThemeProvider>
      </body>
    </html>
  );
}
