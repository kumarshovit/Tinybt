import { Navigate } from "react-router-dom";
import { getUserRole } from "../../utils/auth";
import type { ReactNode } from "react";

const AdminRoute = ({ children }: { children: ReactNode }) => {
  const role = getUserRole();

  if (role !== "Admin") {
    return <Navigate to="/dashboard" />;
  }

  return children;
};

export default AdminRoute;
