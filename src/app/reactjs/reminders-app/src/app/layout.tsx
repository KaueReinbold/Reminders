import './globals.css'

import { Suspense } from 'react'
import type { Metadata } from 'next/types'

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
      <body suppressHydrationWarning={true}>
        <ReminderQueryProvider>
          {/* Suspense required: RemindersContextProvider reads useSearchParams */}
          <Suspense>
            <RemindersContextProvider>
              {children}
            </RemindersContextProvider>
          </Suspense>
        </ReminderQueryProvider>
      </body>
    </html>
  )
}
