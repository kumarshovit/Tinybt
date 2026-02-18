import { Navigate } from "react-router-dom";
import React from "react"; // make sure this line is present

type ProtectedRouteProps = {
  children: React.ReactNode; // safe type for children
};

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const token = localStorage.getItem("token");

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
