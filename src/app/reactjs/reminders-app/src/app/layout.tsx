import './globals.css'

import type { Metadata } from 'next'
// Temporarily disabled due to network restrictions in sandbox environment
// import { Inter } from 'next/font/google'

import { ReminderQueryProvider, RemindersContextProvider } from './hooks'

// const inter = Inter({ subsets: ['latin'] })

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
      <body>
        <ReminderQueryProvider>
          <RemindersContextProvider>
            {children}
          </RemindersContextProvider>
        </ReminderQueryProvider>
      </body>
    </html>
  )
}
