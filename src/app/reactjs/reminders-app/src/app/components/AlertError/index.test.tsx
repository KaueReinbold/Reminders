import { render } from '@testing-library/react';
import { AlertError } from './index';

describe('AlertError', () => {
  it('renders the error message', () => {
    const errorMessage = 'Something went wrong';
    const { getByText } = render(<AlertError error={errorMessage} />);
    expect(getByText(errorMessage)).toBeInTheDocument();
  });

  it('does not render when no error message is provided', () => {
    const { container } = render(<AlertError />);
    expect(container.firstChild).toBeNull();
  });
});
