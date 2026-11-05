import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { isTokenValid } from "../utils/auth";

interface PublicOnlyRouteProps {
  children: ReactNode;
}

const PublicOnlyRoute = ({ children }: PublicOnlyRouteProps) => {
  const authenticated = typeof window !== "undefined" && isTokenValid();

  if (authenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

export default PublicOnlyRoute;
