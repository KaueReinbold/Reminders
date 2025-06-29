'use client';

import React, { useState } from 'react';

import {
  QueryClient,
  QueryClientProvider,
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';

function ReminderQueryProvider({ children }: React.PropsWithChildren) {
  const [client] = useState(new QueryClient());

  return <QueryClientProvider client={client}>{children}</QueryClientProvider>;
}

function useRemindersQueryClient() {
  const queryClient = useQueryClient();

  return queryClient;
}

export {
  ReminderQueryProvider,
  useRemindersQueryClient,
  useMutation,
  useQuery,
};
