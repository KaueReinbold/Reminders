import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';

import { Sidebar } from './index';

const baseProps = {
  view: 'All' as const,
  counts: { All: 5, Today: 1, Upcoming: 2, Done: 2 },
  onSelectView: jest.fn(),
  progress: { pct: 40, caption: '2 of 5 done in the next 7 days' },
};

describe('Sidebar', () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  it('renders all views with counts', () => {
    render(<Sidebar {...baseProps} />);

    expect(screen.getByText('All')).toBeInTheDocument();
    expect(screen.getByText('Today')).toBeInTheDocument();
    expect(screen.getByText('Upcoming')).toBeInTheDocument();
    expect(screen.getByText('Done')).toBeInTheDocument();
    expect(screen.getByText('5')).toBeInTheDocument();
    expect(screen.getByText('1')).toBeInTheDocument();
  });

  it('marks only the active view with aria-current', () => {
    render(<Sidebar {...baseProps} view="Today" />);

    const active = screen.getByRole('button', { current: true });
    expect(active).toHaveTextContent('Today');
  });

  it('calls onSelectView when a view is clicked', () => {
    render(<Sidebar {...baseProps} />);

    fireEvent.click(screen.getByText('Upcoming'));

    expect(baseProps.onSelectView).toHaveBeenCalledWith('Upcoming');
  });

  it('renders the week progress block', () => {
    render(<Sidebar {...baseProps} />);

    expect(screen.getByText('This week')).toBeInTheDocument();
    expect(screen.getByText('40%')).toBeInTheDocument();
    expect(
      screen.getByText('2 of 5 done in the next 7 days'),
    ).toBeInTheDocument();
  });
});
