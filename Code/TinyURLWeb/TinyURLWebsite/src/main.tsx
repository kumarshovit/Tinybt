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

const prerenderedRoutes = [
  "/",
  "/contact",
  "/terms",
  "/privacy-policy",
  "/login",
  "/register",
  "/forgot-password",
];

const isPrerenderedRoute =
  typeof window !== "undefined" &&
  prerenderedRoutes.includes(window.location.pathname);

if (root.hasChildNodes() && isPrerenderedRoute) {
  hydrateRoot(root, app);
} else {
  root.innerHTML = "";
  createRoot(root).render(app);
}
