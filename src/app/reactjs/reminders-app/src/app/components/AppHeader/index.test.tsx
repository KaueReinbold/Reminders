import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';

import { AppHeader } from './index';

describe('AppHeader', () => {
  it('renders title, today date, search, and create button', () => {
    render(
      <AppHeader query="" onQueryChange={jest.fn()} onCreate={jest.fn()} />,
    );

    const todayLabel = new Date().toLocaleDateString('en-US', {
      weekday: 'long',
      month: 'long',
      day: 'numeric',
    });

    expect(screen.getByText('Reminders')).toBeInTheDocument();
    expect(screen.getByText(todayLabel)).toBeInTheDocument();
    expect(screen.getByPlaceholderText('Search reminders')).toBeInTheDocument();
    expect(screen.getByText('New reminder')).toBeInTheDocument();
    expect(screen.queryByText('Demo data')).not.toBeInTheDocument();
  });

  it('reports search input changes', () => {
    const onQueryChange = jest.fn();

    render(
      <AppHeader query="" onQueryChange={onQueryChange} onCreate={jest.fn()} />,
    );

    fireEvent.change(screen.getByPlaceholderText('Search reminders'), {
      target: { value: 'milk' },
    });

    expect(onQueryChange).toHaveBeenCalledWith('milk');
  });

  it('calls onCreate when the new reminder button is clicked', () => {
    const onCreate = jest.fn();

    render(<AppHeader query="" onQueryChange={jest.fn()} onCreate={onCreate} />);

    fireEvent.click(screen.getByText('New reminder'));

    expect(onCreate).toHaveBeenCalled();
  });
});
