import { defineConfig } from 'vite';
import angular from '@analogjs/vite-plugin-angular';
import { join } from 'path';
export default defineConfig({
  plugins: [angular()],
  server: {
    open: true,
    host: true,
    port: 0, // 0 = auto-pick free port
    fs: { strict: false }
  },
  preview: {
    host: true,
    port: 0 // auto-pick free port
  },
  resolve: {
    alias: {
      '@': join(__dirname, 'src')
    }
  },
  build: {
    outDir: 'dist'
  }
});