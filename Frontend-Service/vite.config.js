import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { TUNNEL_URL } from './src/signalr/chatConnection';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  base: './',
  build: {
    outDir: "dist-react",
  },
  server: {
    proxy: {
      "/api": {
        target: TUNNEL_URL,
        changeOrigin: true,
        secure: false
      }
    }
  }
});
