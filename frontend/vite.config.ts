import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Local dev: proxy API calls to the running .NET backend so the
    // frontend can use relative /api/... URLs (same as in the container).
    // Change the target if your backend prints a different port.
    proxy: {
      '/api': 'http://localhost:5001',
    },
  },
})
