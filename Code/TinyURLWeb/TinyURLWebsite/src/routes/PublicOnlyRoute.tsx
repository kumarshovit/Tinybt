import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";

interface PublicOnlyRouteProps {
  children: ReactNode;
}

const PublicOnlyRoute = ({ children }: PublicOnlyRouteProps) => {
  const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;

  if (token) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

export default PublicOnlyRoute;

