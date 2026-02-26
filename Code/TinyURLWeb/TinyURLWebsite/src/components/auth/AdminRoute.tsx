import { Navigate } from "react-router-dom";
import { getUserRole } from "../../utils/auth";

const AdminRoute = ({ children }: { children: JSX.Element }) => {
  const role = getUserRole();

  if (role !== "Admin") {
    return <Navigate to="/dashboard" />;
  }

  return children;
};

export default AdminRoute;
