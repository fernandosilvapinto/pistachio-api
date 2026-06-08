import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      // tudo que começar com /api vai para o backend
      "/api": {
        target: "http://localhost:5150", // URL do seu backend
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
