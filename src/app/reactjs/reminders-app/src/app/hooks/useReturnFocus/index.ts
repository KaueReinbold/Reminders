'use client';

import { useEffect, useRef } from 'react';

// Returns focus to whatever opened the overlay once it closes, so keyboard
// users are not dropped at the top of the page. `enabled` mirrors
// `useEscapeKey`: overlays that render null while closed stay mounted.
export function useReturnFocus(enabled = true): void {
  const trigger = useRef<HTMLElement | null>(null);

  // Captured during render, not in the effect: React applies `autoFocus` on
  // commit, so by effect time the overlay has already taken focus.
  if (enabled && !trigger.current) {
    trigger.current = document.activeElement as HTMLElement | null;
  }

  useEffect(() => {
    if (!enabled) return;

    return () => {
      // The trigger can unmount alongside the overlay: deleting from the sheet
      // closes both, and the deleted reminder's card goes with them. Focusing a
      // detached node silently drops focus, so leave it where it is instead.
      if (trigger.current?.isConnected) trigger.current.focus?.();

      trigger.current = null;
    };
  }, [enabled]);
}
