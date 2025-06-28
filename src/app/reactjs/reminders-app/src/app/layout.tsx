import './globals.css'

import type { Metadata } from 'next'

import { ReminderQueryProvider, RemindersContextProvider } from './hooks'

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
      <body style={{ fontFamily: 'system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Arial, sans-serif' }}>
        <ReminderQueryProvider>
          <RemindersContextProvider>
            {children}
          </RemindersContextProvider>
        </ReminderQueryProvider>
      </body>
    </html>
  )
}
