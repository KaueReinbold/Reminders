'use client';

import { useEffect } from 'react';

// Closes overlays with the Escape key, as the design handoff expects for
// both the create/edit sheet and the delete confirmation.
export function useEscapeKey(onEscape: () => void): void {
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onEscape();
    };

    document.addEventListener('keydown', handleKeyDown);

    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [onEscape]);
}
