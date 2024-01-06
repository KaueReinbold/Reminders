import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import './globals.css'
import QueryProviders from './utils/ApiProvider'

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'Reminders App',
  description: 'A user-friendly application to create, manage, and organize your daily reminders.',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <QueryProviders>
          {children}
        </QueryProviders>
      </body>
    </html>
  )
}
