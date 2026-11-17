import React from "react";
import { createRoot, hydrateRoot } from "react-dom/client";
import { HelmetProvider } from "react-helmet-async";
import App from "./App";
import "./index.css";
import { getPrerenderRoutes } from "./seo/routeRegistry";
import { initializeAnalytics } from "./services/analytics";
import { setupLoadingInterceptors } from "./utils/setupLoadingInterceptors";

if (typeof window !== "undefined") {
  initializeAnalytics();
  setupLoadingInterceptors();
}

const root = document.getElementById("root")!;

const isMsalPopup =
  typeof window !== "undefined" &&
  Boolean(window.opener) &&
  (window.location.hash.includes("code=") ||
    window.location.hash.includes("error=") ||
    window.location.search.includes("code="));

if (isMsalPopup) {
  // Prevent mounting the entire website inside the Microsoft popup window
  root.innerHTML =
    '<div style="display:flex;align-items:center;justify-content:center;height:100vh;background:#0B1120;color:#94a3b8;font-family:system-ui,sans-serif;font-size:14px;">Completing sign-in, please wait...</div>';

  import("./services/msalService").then(({ getMsalInstance }) => {
    getMsalInstance();
  });
} else {
  const app = (
    <React.StrictMode>
      <HelmetProvider>
        <App />
      </HelmetProvider>
    </React.StrictMode>
  );

  const prerenderedPaths = new Set(getPrerenderRoutes());
  const currentPath = typeof window !== "undefined" ? window.location.pathname : "/";
  const normalizedCurrent = currentPath.endsWith("/") || currentPath === "/login" ? currentPath : `${currentPath}/`;
  const isPrerenderedRoute = prerenderedPaths.has(currentPath) || prerenderedPaths.has(normalizedCurrent);

  if (root.hasChildNodes() && isPrerenderedRoute) {
    hydrateRoot(root, app);
  } else {
    root.innerHTML = "";
    createRoot(root).render(app);
  }
}
