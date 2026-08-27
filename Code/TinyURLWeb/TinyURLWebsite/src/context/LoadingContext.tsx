import React, { createContext, useContext, useState, useEffect, useCallback, useRef } from "react";

interface LoadingContextType {
  isLoading: boolean;
  loadingCount: number;
  message: string;
  startLoading: (message?: string) => () => void;
  stopLoading: () => void;
  withLoading: <T>(promise: Promise<T>, message?: string) => Promise<T>;
}

const LoadingContext = createContext<LoadingContextType>({
  isLoading: false,
  loadingCount: 0,
  message: "",
  startLoading: () => () => {},
  stopLoading: () => {},
  withLoading: (p) => p,
});

// Global listeners for non-React code (e.g. axios & fetch interceptors)
type LoadingListener = (count: number, message: string) => void;
const listeners = new Set<LoadingListener>();
let globalLoadingCount = 0;
let globalLoadingMessage = "";

export const incrementGlobalLoading = (msg?: string) => {
  globalLoadingCount++;
  if (msg) {
    globalLoadingMessage = msg;
  }
  listeners.forEach((l) => l(globalLoadingCount, globalLoadingMessage));
};

export const decrementGlobalLoading = () => {
  globalLoadingCount = Math.max(0, globalLoadingCount - 1);
  if (globalLoadingCount === 0) {
    globalLoadingMessage = "";
  }
  listeners.forEach((l) => l(globalLoadingCount, globalLoadingMessage));
};

export const LoadingProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [loadingCount, setLoadingCount] = useState(0);
  const [message, setMessage] = useState("");
  const [debouncedIsLoading, setDebouncedIsLoading] = useState(false);
  const debounceTimerRef = useRef<number | null>(null);

  useEffect(() => {
    const handleGlobalUpdate: LoadingListener = (count, msg) => {
      setLoadingCount(count);
      if (msg) setMessage(msg);
    };

    listeners.add(handleGlobalUpdate);
    return () => {
      listeners.delete(handleGlobalUpdate);
    };
  }, []);

  // Debounce loading indicator slightly (100ms) to avoid visual flickering on instant responses
  useEffect(() => {
    if (loadingCount > 0) {
      if (!debouncedIsLoading) {
        debounceTimerRef.current = window.setTimeout(() => {
          setDebouncedIsLoading(true);
        }, 120);
      }
    } else {
      if (debounceTimerRef.current !== null) {
        clearTimeout(debounceTimerRef.current);
        debounceTimerRef.current = null;
      }
      setDebouncedIsLoading(false);
      setMessage("");
    }

    return () => {
      if (debounceTimerRef.current !== null) {
        clearTimeout(debounceTimerRef.current);
      }
    };
  }, [loadingCount]);

  const startLoading = useCallback((msg?: string) => {
    incrementGlobalLoading(msg);
    return () => {
      decrementGlobalLoading();
    };
  }, []);

  const stopLoading = useCallback(() => {
    decrementGlobalLoading();
  }, []);

  const withLoading = useCallback(async <T,>(promise: Promise<T>, msg?: string): Promise<T> => {
    const stop = startLoading(msg);
    try {
      return await promise;
    } finally {
      stop();
    }
  }, [startLoading]);

  return (
    <LoadingContext.Provider
      value={{
        isLoading: debouncedIsLoading,
        loadingCount,
        message,
        startLoading,
        stopLoading,
        withLoading,
      }}
    >
      {children}
    </LoadingContext.Provider>
  );
};

export const useLoading = () => useContext(LoadingContext);
export default LoadingContext;
