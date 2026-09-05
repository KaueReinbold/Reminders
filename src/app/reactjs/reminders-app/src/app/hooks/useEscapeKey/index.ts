'use client';

import { useEffect } from 'react';

// Closes overlays with the Escape key, as the design handoff expects for
// both the create/edit sheet and the delete confirmation. `enabled` keeps the
// listener off while the overlay is closed, so Escape cannot reopen it.
export function useEscapeKey(onEscape: () => void, enabled = true): void {
  useEffect(() => {
    if (!enabled) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onEscape();
    };

    document.addEventListener('keydown', handleKeyDown);

    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [onEscape, enabled]);
}
