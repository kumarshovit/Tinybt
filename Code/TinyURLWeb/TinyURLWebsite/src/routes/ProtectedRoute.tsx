import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { isTokenValid } from "../utils/auth";

const ProtectedRoute = ({ children }: { children: ReactNode }) => {
  const authenticated = typeof window !== "undefined" && isTokenValid();

  if (!authenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

export default ProtectedRoute;
