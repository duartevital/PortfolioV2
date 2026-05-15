import type { Config } from 'tailwindcss'

export default {
  content: [
    './app/**/*.{vue,ts,tsx}',
    './components/**/*.{vue,ts,tsx}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
  ],
  theme: {
    extend: {
      colors: {
        bg:      'var(--color-bg)',
        surface: 'var(--color-surface)',
        border:  'var(--color-border)',
        text:    'var(--color-text)',
        muted:   'var(--color-muted)',
        // accent: TBD Phase 2
      },
    },
  },
  plugins: [],
} satisfies Config
