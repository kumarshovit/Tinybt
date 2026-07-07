import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

export default defineConfig(({ isSsrBuild }) => ({
  plugins: [react(), tailwindcss()],
  build: {
    rollupOptions: isSsrBuild
      ? undefined
      : {
          output: {
            manualChunks: {
              "vendor-react": ["react", "react-dom", "react-router-dom"],
              "vendor-charts": ["recharts"],
              "vendor-select": ["react-select", "react-select/creatable"],
            },
          },
        },
  },
}));
