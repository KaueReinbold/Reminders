import { render } from '@testing-library/react';
import { RemindersContextProvider } from '.';
import Home from '@/app/page';

jest.mock(
  'next/navigation',
  require('@/app/util/testMocks').jestMocks['next/navigation'],
);
jest.mock('@/app/api', require('@/app/util/testMocks').jestMocks['@/app/api']);
jest.mock(
  '@/app/hooks',
  require('@/app/util/testMocks').jestMocks['@/app/hooks'],
);

describe('RemindersContextProvider Tests', () => {
  it('should render children with default state', async () => {
    render(
      <RemindersContextProvider>
        <Home />
      </RemindersContextProvider>,
    );
  });

  it.todo('should update context values on successful createReminder');

  it.todo('should handle errors on createReminder');

  it.todo('should update context values on successful updateReminder');

  it.todo('should handle errors on updateReminder');

  it.todo('should handle errors on deleteReminder');
});
