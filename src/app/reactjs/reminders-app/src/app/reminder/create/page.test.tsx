import { render, screen, fireEvent } from '@testing-library/react';
import Create from './page';

jest.mock(
  'next/navigation',
  require('@/app/util/testMocks').jestMocks['next/navigation'],
);
jest.mock(
  '@/app/hooks',
  require('@/app/util/testMocks').jestMocks['@/app/hooks'],
);

describe('Create Component', () => {
  afterEach(() => {
    jest.restoreAllMocks();
  });

  it('should render Create component', () => {
    render(<Create />);

    expect(screen.getByText('Create')).toBeInTheDocument();
    expect(screen.getByText('Back')).toBeInTheDocument();
  });

  it('should call onCreateReminder and handles form submission', async () => {
    render(<Create />);

    fireEvent.click(screen.getByText('Create'));

    await screen.findByText('Back');

    expect(
      require('@/app/hooks').useRemindersContext().onCreateReminder,
    ).toHaveBeenCalled();
    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/',
    );
  });

  it('should handle Back button click', () => {
    render(<Create />);

    fireEvent.click(screen.getByText('Back'));

    expect(require('next/navigation').useRouter().push).toHaveBeenCalledWith(
      '/',
    );
  });
});
