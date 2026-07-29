import './globals.css'

import { Suspense } from 'react'
import { DM_Sans, Instrument_Serif } from 'next/font/google'
import type { Metadata } from 'next/types'

const instrumentSerif = Instrument_Serif({
  weight: '400',
  style: ['normal', 'italic'],
  subsets: ['latin'],
  variable: '--font-instrument-serif',
})

const dmSans = DM_Sans({
  subsets: ['latin'],
  variable: '--font-dm-sans',
})

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
    <html lang="en" className={`${instrumentSerif.variable} ${dmSans.variable}`}>
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
