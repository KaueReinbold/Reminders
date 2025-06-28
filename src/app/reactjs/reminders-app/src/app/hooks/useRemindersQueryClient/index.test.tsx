import { act, render, screen } from '@testing-library/react';
import { QueryClient, useQueryClient } from '@tanstack/react-query';
import {
  ReminderQueryProvider,
  useRemindersQueryClient,
  useMutation,
  useQuery,
} from '.';
import { ReactNode } from 'react';

jest.mock('@tanstack/react-query', () => ({
  ...jest.requireActual('@tanstack/react-query'),
  useQueryClient: jest.fn(),
  QueryClientProvider: ({ children }: { children: any }) => children,
  QueryClient: jest.fn(),
}));

const TestComponent = () => {
  useRemindersQueryClient();

  return <></>;
};

describe('ReminderQueryProvider', () => {
  async function renderProvider(children: ReactNode) {
    await act(async () => {
      render(<ReminderQueryProvider>{children}</ReminderQueryProvider>);
    });
  }

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('render children with QueryClientProvider', async () => {
    await renderProvider(<div>Test Child</div>);

    const childElement = screen.getByText('Test Child');
    expect(childElement).toBeInTheDocument();
  });

  it('use QueryClientProvider with a new QueryClient', async () => {
    await renderProvider(<div>Test Child</div>);

    expect(QueryClient).toHaveBeenCalledTimes(1);
  });

  it('use useQueryClient hook', () => {
    render(<TestComponent />);

    expect(useQueryClient).toHaveBeenCalledTimes(1);
  });

  it('should method be defined', () => {
    expect(useQuery).toBeDefined();
    expect(useMutation).toBeDefined();
  });
});
