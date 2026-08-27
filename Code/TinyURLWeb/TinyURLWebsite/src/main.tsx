import React from "react";
import { createRoot, hydrateRoot } from "react-dom/client";
import { HelmetProvider } from "react-helmet-async";
import App from "./App";
import "./index.css";

import { initializeAnalytics } from "./services/analytics";
import { setupLoadingInterceptors } from "./utils/setupLoadingInterceptors";

if (typeof window !== "undefined") {
  initializeAnalytics();
  setupLoadingInterceptors();
}

const app = (
  <React.StrictMode>
    <HelmetProvider>
      <App />
    </HelmetProvider>
  </React.StrictMode>
);

const root = document.getElementById("root")!;

if (root.hasChildNodes()) {
  hydrateRoot(root, app);
} else {
  createRoot(root).render(app);
}
