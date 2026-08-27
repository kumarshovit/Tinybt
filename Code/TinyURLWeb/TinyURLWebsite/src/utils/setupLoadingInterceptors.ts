import axios from "axios";
import { incrementGlobalLoading, decrementGlobalLoading } from "../context/LoadingContext";

let interceptorsInitialized = false;

export function setupLoadingInterceptors() {
  if (interceptorsInitialized || typeof window === "undefined") {
    return;
  }
  interceptorsInitialized = true;

  // 1. Axios Request / Response Interceptor on default axios
  axios.interceptors.request.use(
    (config) => {
      // Allow opting out of global loading via headers
      if (!config.headers?.["x-skip-loading"]) {
        incrementGlobalLoading();
      }
      return config;
    },
    (error) => {
      decrementGlobalLoading();
      return Promise.reject(error);
    }
  );

  axios.interceptors.response.use(
    (response) => {
      if (!response.config.headers?.["x-skip-loading"]) {
        decrementGlobalLoading();
      }
      return response;
    },
    (error) => {
      if (!error.config?.headers?.["x-skip-loading"]) {
        decrementGlobalLoading();
      }
      return Promise.reject(error);
    }
  );

  // 2. Global Fetch Interceptor
  const originalFetch = window.fetch;
  window.fetch = async function (...args) {
    const [, config] = args;
    const headers = (config?.headers || {}) as Record<string, string>;
    const skipLoading = headers["x-skip-loading"] === "true";

    if (!skipLoading) {
      incrementGlobalLoading();
    }

    try {
      const response = await originalFetch.apply(this, args);
      return response;
    } finally {
      if (!skipLoading) {
        decrementGlobalLoading();
      }
    }
  };
}
